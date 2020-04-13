using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FileWatcher.Abstracts.Contracts;
using Newtonsoft.Json;
using Serilog;

namespace FileWatcher.Core {
  public class Dispatcher : IDispatcher {
    private const int COOLDOWN_TIME_FOR_THREAD_IN_MS = 3000;
    private ILogger logger;
    private Dictionary<Type, List<Action<object>>> consumers = new Dictionary<Type, List<Action<object>>>();
    private List<Action> consumersToDispose = new List<Action>();
    private ConcurrentQueue<object> queue = new ConcurrentQueue<object>();
    private List<Task> workers = new List<Task>();
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public Dispatcher(ILogger logger, int numWorkers = 1) {
      this.logger = logger;
      numWorkers = Math.Max(1, numWorkers);
      initializeWorkers(numWorkers);
    }

    private void initializeWorkers(int numWorkers) {
      var taskFactory = new TaskFactory(cancellationTokenSource.Token);
      while (numWorkers-- > 0) {
        workers.Add(taskFactory.StartNew(sendMessageToConsumer));
      }
    }

    private async Task sendMessageToConsumer() {
      while (!cancellationTokenSource.IsCancellationRequested) {
        if (queue.TryDequeue(out object messageToProcess)) {
          var type = messageToProcess.GetType();
          var json = JsonConvert.SerializeObject(messageToProcess);
          if (consumers.ContainsKey(type)) {
            logger.Information("starting: processing message {0}", json);
            foreach (var item in consumers[type]) {
              try {
                item(messageToProcess);
              } catch (Exception ex) {
                logger.Error("One consumer failed to process message(type={0}) {1} with error {2}", type.ToString(), json, ex.ToString());
              }
            }
            logger.Information("finished: processing message {0}", json);
          } else {
            logger.Error("Application is not able to dispatch message: {0}", json);
          }
        } else {
          await Task.Delay(COOLDOWN_TIME_FOR_THREAD_IN_MS);
        }
      }
    }

    public void DispatchMessage(object msg) {
      logger.Debug("Dispatcher receiving message: {0}", JsonConvert.SerializeObject(msg));
      queue.Enqueue(msg);
    }

    public IDispatcher RegisterConsumer<T>(IMesssageConsumer<T> consumer) {
      var consumerType = consumer.GetType();
      var messageType = typeof(T);
      logger.Information("starting: adding consumer {0} for type {1}", consumerType.ToString(), messageType.ToString());
      addConsumer(consumer, messageType);
      addConsumerToDispose(consumer, consumerType.ToString());
      logger.Information("finished: adding consumer {0} for type {1}", consumerType.ToString(), messageType.ToString());
      return this;
    }

    private void addConsumerToDispose<T>(IMesssageConsumer<T> consumer, string type) {
      consumersToDispose.Add(() => {
        try {
          logger.Information(string.Format("starting: disposing message consumer, {0}", type));
          consumer.Dispose();
          logger.Information(string.Format("finished: disposing message consumer, {0}", type));
        } catch (Exception ex) {
          logger.Error("An error occurred when disposing {0}", type);
        }
      });
    }

    private void addConsumer<T>(IMesssageConsumer<T> consumer, Type messageType) {
      if (!consumers.ContainsKey(messageType)) {
        consumers.Add(messageType, new List<Action<object>>());
      }
      consumers[messageType].Add((object input) => {
        var json = JsonConvert.SerializeObject(input);
        try {
          logger.Information(string.Format("starting: Processing message: {0}", json));
          consumer.ConsumeMessage((T)input);
          logger.Information(string.Format("finished: Processing message: {0}", json));
        } catch (Exception ex) {
          logger.Error(string.Format("Error when processing message <<{0}>> with error <<{1}>>", json, ex.ToString()));
        }
      });
    }

    public void Dispose() {
      logger.Information("starting: Disposing message consumers");
      try {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        foreach (var item in consumersToDispose) {
          item();
        }
      } catch (Exception ex) {
        logger.Error(string.Format("Error when disposing consumer message: {0}", ex.ToString()));
      }
      logger.Information("finished: Disposing message consumers");
    }
  }
}

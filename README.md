# File Watcher
> Windows service to notify for changes in a folder

This project consists of a console application which uses TopShelf to deploy a windows service.
The project is used to watch for changes in a folder and notify those changes with a toast notification.

## Installing / Getting started

Follow the next steps to install the windows service:

* Open a command line in the root of the project and type the following command

```shell
FileExplorer.exe install
```

You will see a bunch of logging info in the console.
```shell
2017-08-14 22:52:51.6685|INFO|Starting Hangfire Server
2017-08-14 22:52:51.6730|INFO|Using job storage: 'Hangfire.MemoryStorage.MemoryStorage'
2017-08-14 22:52:51.6730|INFO|Using the following options for Hangfire Server:
2017-08-14 22:52:51.6730|INFO|    Worker count: 1
2017-08-14 22:52:51.6730|INFO|    Listening queues: 'default'
2017-08-14 22:52:51.6730|INFO|    Shutdown timeout: 00:00:15
2017-08-14 22:52:51.6730|INFO|    Schedule polling interval: 00:00:15
2017-08-14 22:52:51.6915|DEBUG|Background process 'BackgroundProcessingServer' started.
2017-08-14 22:52:51.8385|DEBUG|Background process 'ServerHeartbeat' started.
2017-08-14 22:52:51.8385|DEBUG|Background process 'ServerWatchdog' started.
2017-08-14 22:52:51.8385|DEBUG|Background process 'Composite C1 Records Expiration Manager' started.
2017-08-14 22:52:51.8385|DEBUG|Background process 'Counter Table Aggregator' started.
2017-08-14 22:52:51.8385|DEBUG|Background process 'Worker #f821cf10' started.
2017-08-14 22:52:51.8430|DEBUG|Background process 'DelayedJobScheduler' started.
2017-08-14 22:52:51.8430|DEBUG|Background process 'RecurringJobScheduler' started.
Configuration Result:
[Success] Name FileWatcher
[Success] Description Program to watch file changes in a folder
[Success] ServiceName FileWatcher
Topshelf v4.0.0.0, .NET Framework v4.0.30319.42000

Running a transacted installation.

Beginning the Install phase of the installation.
Installing FileWatcher service
Installing service FileWatcher...
Service FileWatcher has been successfully installed.

The Install phase completed successfully, and the Commit phase is beginning.

The Commit phase completed successfully.

The transacted install has completed.
```

It will be successfully installed if you see the last lines of the previous example

The process for uninstalling the service is to open a command line in the root folder of the project and type the following:

```shell
FileWatcher.exe uninstall
```

And you will see a bunch of logs like this:

```shell
2017-08-14 22:56:23.2861|INFO|Starting Hangfire Server
2017-08-14 22:56:23.2861|INFO|Using job storage: 'Hangfire.MemoryStorage.MemoryStorage'
2017-08-14 22:56:23.2861|INFO|Using the following options for Hangfire Server:
2017-08-14 22:56:23.2861|INFO|    Worker count: 1
2017-08-14 22:56:23.2861|INFO|    Listening queues: 'default'
2017-08-14 22:56:23.2861|INFO|    Shutdown timeout: 00:00:15
2017-08-14 22:56:23.2861|INFO|    Schedule polling interval: 00:00:15
2017-08-14 22:56:23.3106|DEBUG|Background process 'BackgroundProcessingServer' started.
2017-08-14 22:56:23.4201|DEBUG|Background process 'ServerHeartbeat' started.
2017-08-14 22:56:23.4201|DEBUG|Background process 'ServerWatchdog' started.
2017-08-14 22:56:23.4201|DEBUG|Background process 'Composite C1 Records Expiration Manager' started.
2017-08-14 22:56:23.4221|DEBUG|Background process 'Counter Table Aggregator' started.
2017-08-14 22:56:23.4221|DEBUG|Background process 'Worker #5f402056' started.
2017-08-14 22:56:23.4221|DEBUG|Background process 'DelayedJobScheduler' started.
2017-08-14 22:56:23.4221|DEBUG|Background process 'RecurringJobScheduler' started.
Configuration Result:
[Success] Name FileWatcher
[Success] Description Program to watch file changes in a folder
[Success] ServiceName FileWatcher
Topshelf v4.0.0.0, .NET Framework v4.0.30319.42000


The uninstall is beginning.
Uninstalling FileWatcher service
Service FileWatcher is being removed from the system...
Service FileWatcher was successfully removed from the system.

The uninstall has completed.
```

## Developing

In order to start developing follow the next steps:

Clone the project

```shell
git clone https://github.com/gus-sosa/FileWatcher.git
```

And the open the FileWatcher.sln with Visual Studio and from there you will be able to compile and execute the project

## Features

* Show toast notifications when it detects changes in folders

## Configuration

Here you should write what are all of the configurations a user can enter when
using the project.

## Contributing

You will be welcome to contribute in this project with recomendations, design or programming.

This is just for training and anything you can teach me it is good for me.

## Licensing

The code in this project is licensed under MIT license.
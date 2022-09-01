# Release notes

# 12.0.0

* Breaking change: ```IOperationResult``` inherits from ```IDynamic``` instead of ```IExpandoBase```.
* Breaking change: ```IOperationResult.GetAwaiter()``` is removed, instead use ```await IOperation.AsTask()```.
* NEW: Added ```IOperationMessage.Exception : Exception?```.
* NEW: Added ```IOperationMessage.IsError()```: boolean and ```IOperationMessage.IsWarning(): boolean```. They check the message for implementation of ```ISeverityQualifiedNotification``` and the enclosed ```Exception```.
* Breaking change: removed ```IOperationResult.Exceptions```, now they are included in ```Messages```.
* NEW: Added ```IOperationResult``` extension methods ```HasErrors()``` and ```HasWarnings()```, ```Errors()``` and ```Warnings()```.
* NEW: Added ```OperationException(Exception)```.
* Changed ```OperationMessage.ToString()``` to include the exception information, if provided.

# Other resources
* Please check https://github.com/kephas-software/kephas/releases for the change log.
* Also check the documentation and the samples from https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

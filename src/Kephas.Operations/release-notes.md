# 12.0.0

* Breaking change: ```IOperationResult``` inherits from ```IDynamic``` instead of ```IExpandoBase```.
* Breaking change: ```IOperationResult.GetAwaiter()``` is removed, instead use ```await IOperation.AsTask()```.

# Other resources
Please check https://github.com/kephas-software/kephas/releases for the change log.
Also check the documentation and the samples from https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

﻿# 12.0.0

* Breaking change: Removed ```ExportFactory(Func<Tuple<TContract, Action>> factory)``` constructor.
* Breaking change: Removed ```IExportFactory.CreateExport```. Replace with ```IExportFactory.CreateExportedValue```.
* Breaking change: Removed ```IExport``` and ```Export```. No replacement. Reason: obsolete artifact to support ``System.Composition``.

# Other resources
* Change log: https://github.com/kephas-software/kephas/releases.
* Documentation and samples: https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.
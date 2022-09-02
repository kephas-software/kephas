# Release notes

# 12.0.0

* Breaking change: provided the ```ITemplatingEngine.ProcessAsync``` default implementation instead of ```Process```.
* Added support for property path and formatting in ```InterpolationTemplatingEngine```.
* Breaking change: in ``TemplateProcessorExtensions``, method ``ProcessWithFile`` was renamed to ``ProcessFile``, and ``Interpolate`` was renamed to ``Process``.
* Breaking change: in ``ITemplateProcessor``, the method ``Process`` must be implemented and ``ProcessAsync`` gets a default implementation by calling the sync ``Process`` method.
 
# Other resources
* Please check https://github.com/kephas-software/kephas/releases for the change log.
* Also check the documentation and the samples from https://github.com/kephas-software/kephas/wiki and https://github.com/kephas-software/kephas/tree/master/Samples.

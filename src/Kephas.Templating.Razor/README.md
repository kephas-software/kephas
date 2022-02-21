# Templating.Razor

## Introduction
This package provides the implementation of templating using Razor syntax and *.cshtml files.
The ```cshtml``` template kind is handled by the ```RazorTemplatingEngine```.

## Additional services

### ```IMetadataReferenceManager```
This internal service is used to provide ```MetadataReference```s out of a list of assemblies.

### ```IRazorProjectFileSystemProvider```
Base on the template and the processing context, this service should provide a ```RazorProjectFileSystem``` used by the renderer.

The default implementation ```SimpleRazorProjectFileSystemProvider``` uses a simple strategy which handles a single template file without any additional template references.
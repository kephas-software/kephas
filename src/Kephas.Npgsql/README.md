# PostgreSQL connectivity

## Introduction

Npgsql provides the .NET connectivity to PostgreSQL databases. To leverage integration with Kephas, the ```NpgsqlAppLifecycleBehavior```
sets this up by setting the logging provider. However, for this to work, either use the ```Kephas.Application``` package (recommended), where the behavior will be invoked automatically upon aplication bootstrap,
or invoke the ```NpgsqlAppLifecycleBehavior.BeforeAppInitializeAsync``` manually.
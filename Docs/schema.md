**Disclaimer:** This project is a work in progress.  I haven't finished everything.
Sorry if you get some random exceptions/errors

## Parse Yang/Yin modules

**Important:** Not all YANG functionality is supported.

The first step is to load a `ISyntaxSchema`.  This contains all of the modules
(and later, submodules, once I implement that.) to consider for a "Resolved" schema
["Resolved" schema](#resolving-the-schema).  

In this stage, the schema is a direct representation of the YANG/YIN files.

> TODO: I'll eventually add the capability to consume a `modules-state` data tree 
(from the `ietf-yang-library` module), rather than specifying the modules/features here.

### Example

```c#
var syntaxSchema = SchemaLoader.LoadSyntaxSchema(SchemaSettings.Default with
{
    SearchDirectory = new DirectoryInfo(@"C:\Path\To\Yang\Modules"),
    ModuleNames = new[]
    {
        "openconfig-system",
        "openconfig-platform",
    },
});
```

## Resolving the Schema

In order to do anything useful with the schema, you'll need to resolve it.
A resolved schema implements `IResolvedSchema`.

Resolving the schema will locate types/groupings/etc..

For example, in a `ISyntaxSchema`, a `leaf` has a `type` statement, whose 
argument is the _name_ of the data type for the `leaf`.  In an `IResolvedSchema`, 
a `IResolvedSchemaLeaf` has a `Type` property (`IUsableType`), which is an object
that provides validation of the type restrictions (such as `pattern`, `length`, `range`, etc.)

> **Note:** A resolved schema is read-only.  If you want to modify the schema,
> you'll need to edit the YANG/YIN files, and then go through the process again.

### Example

```c#
var resolvedSchema = syntaxSchema.Resolve();
```

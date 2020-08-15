# BinaryFormatDataStructure
.NET Library for parsing [MS-NRBF](https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-nrbf/75b9fe09-be15-475f-85b8-ae7b7558cfe5) streams.

Allows the reading of the output that is typically created from BinaryFormatter without the security concerns of automatically instantiating unsafe classes. 
NRBFReader deserializes primitive types (Boolean, Byte, Char, Double, Int16, Int32, Int64, SByte, Single, UInt16, UInt32, Uint64, Decimal) as well as TimeSpan and DateTime. Primitive arrays are also deserialized.
Other class and object information is returned in a BinaryObject type for the caller to evaluate.


## What is MS-NRBF?

MS-NRBR or Binary Format Data Structure is a series of data records that describe and object graph and its data. It contains both forward and backward looking record references and can represent cyclical data. Using BinaryFormatter to re-read this stream is generally considered to be unsafe is it will create any object specified in the record with unvalidated data.


## Quick start

Here is a quick sample to get you started. However if you are deserializing third part or system objects it is often useful just to inspect the result of ReadStream.

```c#
using BinaryFormatDataStructure;
...
// This example assumes an root object but it also could be an array.
// The object type should be already known if you are deserializing trusted data
// but it can also be determined from BinaryObject properties.
var result = (BinaryObject)NRBFReader.ReadStream(stream);
var document = new Document(result);
...
public class Document
{
    ...
    public Document(BinaryObject model)
    {
        // Optional but if other objects in this deserialized graph have a reference back to this object as
        // a "parent" it is useful to keep a record of the created object.
        model.ReferenceObject = this;

        _width = (int)model["_width"];
        _height = (int)model["_height"];

        if (model.ContainsKey("_depth"))
        {
            // Deserialize versioned property
            _depth = (int)model["_depth"];
        }

        var childModel = (BinaryObject)model["child"];
        // TODO: create child object and continue graph deserialization
    }
}
```

## Installing

Just install the [BinaryFormatDataStructure NuGet package](https://www.nuget.org/packages/BinaryFormatDataStructure):

```
PM> Install-Package BinaryFormatDataStructure
```
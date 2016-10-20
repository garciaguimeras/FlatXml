# FlatXml

### What is FlatXml?

* XML for people who doesn't like XML
* XML for flat people

### Why use FlatXml instead of XML or JSON?

* Because is new, and using new things is fun!

### How to write a fxml file

FlatXml can be explained in a few recipes.

#### First recipe: Use </> no more
Write
```
filter include="*.class" exclude="*.html"
```
instead of
```
<filter include="*.class" exclude="*.html" />
```

#### Second recipe: Use {}
Write
```
task name="createFile"
{
	create filename="newfile.txt"
}
```
instead of
```
<task name="createFile">
	<create filename="newfile.txt" />
</task>
```

#### Third recipe: Use the value attribute
Write
```
property name="base.dir" value="/home/user"
```
instead of
```
<property name="base.dir">/home/user</property>
```

#### Fourth recipe: Define properties with $
Write
```
$base.dir /home/user
```
as a shortcut for
```
property name="base.dir" value="/home/user"
```

#### Fifth recipe: Define meta attributes with @
Write
```
@author Noel
@version 0.1
@draft
```
or
```
meta-attr name="author" value="Noel"
meta-attr name="version" value="0.1"
meta-attr name="draft"
```
to define meta attributes.

#### Sixth recipe: Comment your code
```
# This is a comment
```

#### Last steps

* Mix it during a few hours
* Add some tomato sauce
* Bake it on the oven
* After 30 minutes you'll have a fxml file ready to eat. Enjoy it!

### Serialization and deserialization

#### How to deserialize?

```
string inFilename = "input.fxml";
Deserializer d = new Deserializer();
IEnumerable<FXmlElement> elements = d.Deserialize(File.OpenRead(inFilename));
if (elements == null)
{
	PrintErrors(d.Errors);
	return;
}
```

#### How to serialize?

```
IEnumerable<FXmlElement> elements;
...
string outFilename = "output.fxml";
Serializer s = new Serializer();
Stream stream = s.Serialize(elements);
using (FileStream file = File.OpenWrite(outFilename))
{
	stream.CopyTo(file);
}
```

# Adrastea
A small highly extensible data transformation library written in .NET

# Core Interfaces

The following interfaces are exposed, along with several useful implementations of them.

### Transformer
Defines a series of transformation steps to be applied to an input. The transformation steps will be chained togehter, so the output of one transformation forms the input to the next.

### Transformation
A single step that can be applied by a transformer. There may be multiple transformation steps implemented by a single transformer.

A single transformation can also contain additional transformers that can render different parts of a document.

### TransformingContext
The context that is passed from one transformation to the next by a transformer. This is a simple container that allows more useful interfaces to be retrieved.

### Matcher
Associated with a transformer, it decides whether or not a transformer should be executed for particular input. A hierarchy of matchers can be created if a transformation step needs to run for multiple conditions.

### MatchingContext
The context of the data that is passed by the matchers to determine whether an associated transformer should run. This is a simple container that allows more useful interfaces to be retrieved.

# Customization

Additional implementations of any of these interfaces can be easily injected, along with custom event handling.

See the FileEntryTransformerFactory constructor for an example of how custom object factories can be injected, and events can be customized.

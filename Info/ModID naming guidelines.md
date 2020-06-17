# ModID is an identifier that allows Common framework to differentiate content/functionality that is provided by said mod from others.

#### ModID name should only use ASCII letters/numbers in it, otherwise there might be some encoding errors somewhere, where we had no idea that they might occur in a first place.

#### Most of the ModID looks like 'Author.ShortName', or if you want a real example like this 'Unary.Common'.

#### ModID is used in order to distinguish ModID entries in Common Systems. ModID entries might look something like this: 'Unary.Common.Shared.ConsoleSys'. So, for example:

### 'Unary.Common.Something.Something.Test'

#### Will be interpreted by framework as:

#### 'Unary.Common' - ModID
#### 'Something.Something' - Part of some sort of a tree
#### 'Test' - Target entry of 'Unary.Common.Something.Something'

#### But, there are exceptions to this rule in order to resolve potential ModID collisions in the future
#### You should NOT use ModID as such:

#### 'Unary.[...]' ('Unary.Common', 'Unary.Recusant', etc) This is reserved for our own usage in order to distinguish official mods of the game.

#### 'System', 'Godot', 'mscorlib' These ModIDs are reserved so that C#s types/assemblies will be accessable through AssemblySys.

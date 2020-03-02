ModID is an identifier that allows Common framework to differentiate content/functionality that is provided by said mod from others.

ModID name should only use ASCII letters/numbers in it, otherwise there might be some encoding errors somewhere, where
we had no idea that they might occur in a first place.

Most of the ModID looks like 'Author_ShortName', or if you want a real example like this 'Unary_Common'.

ModID is used in order to distinguish ModID entries in Common Systems. ModID entries might
look something like this: 'Unary_Common.Shared.ConsoleSys'. You might ask yourself a question about
why are we not using a dot instead of an underscore, but that is due to a nature of Common expecting 
to have ModID before the first dot in order to process content entries properly. So, for example:

'Unary_Common.Something.Something.Test'

Will be interpreted by framework as:

'Unary_Common' - ModID
'Something.Something' - Part of some sort of a tree
'Test' - Target entry of 'Unary_Common.Something.Something'

But, there are exceptions to this rule in order to resolve potential ModID collisions in the future
You should NOT use ModID as such:

'Unary_[...]' ('Unary_Common', 'Unary_Recusant', etc) This is reserved for our own usage 
in order to distinguish official mods of the game.

'System', 'Godot', 'mscorlib' These ModIDs are reserved so that C#s types/assemblies will be accessable through AssemblySys.
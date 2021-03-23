# MP_EF_NiclasBlomberg
 
The second mini project for Lexicon: Refactoring the asset tracker from MP1 to work with EF Core.

This mini project also uses some code from my console UI project, which can be found here https://github.com/lambda-snail/SCLI.

# Changes

The following things have changed from the MP1 version:

* The Repository classes now use EF Core under the hood to talk to a database.
* Repository classes now support the full CRUD set of methods.
* Asset and Office classes have been moved to a Model layer. This layer also includes interfaces for allownig the repositories to talk to the Main Layer.

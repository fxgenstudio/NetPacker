
## TODO

* Remove classes 


## ROADMAP
* inline functions that are used once or twice (no entry in the metadata table + same bytecode -> good compression)
* replace pinvoke calls by the 'calli' opcode
* replace memcpy/memset/... by cpblk/initblk
* replace property getter/setters by functions
* merge classes with only static fields/fns/... into 1
* remove useless attributes
* remove/change names of types/fns/... for better compression
* simplify MD tables: make everyting public, no 'static this'/'extension' methods, avoid generics & inheritance, ...
* use small ldi, br, ... opcodes (like br.s) when possible (dnlib does this for you: linky)

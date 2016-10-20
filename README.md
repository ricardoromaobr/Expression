Expression
==========
An expression library that solver math expression

Just clone and compile the soluction and you will have a dll: **Expression.dll**

:smiley:

Example
-------

Simple expression

```cs
var resolver = new Resolver("a * 2");
resolver ["a"] = 5; 
var result = resolver.SolverExpression  ();
```

Logical expression 

```cs

var resolver = new Resolver ("a * 2 > b - 2 (b * a)"); 
resolver ["a"] = 5; 
resolver ["b"] = 10;

var result = resolver.SolverExpression (); 

// result will be  0 if false an 1 if true;  

```


# exemplo 3 

Key words
---------

log10 -

log -

log2 - 

sin - 

cos - 

tan - 

Constants 
---------

pi - PI constant
e - 

! - Factorial


MORE TO COME!

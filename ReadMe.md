
## NetPacker

###  Description
 NetPacker optimize C# application size in order to create Intro64k for demo scene in full C#. (see www.Pouet.net)
 It's a try and I need help from others developpers because I doesn't have a lot of time for Open source Project.
  
 ILOptimizer class remove 'not used methods' only for moment.
 We need to do the same thing for fields and classes.
 

 Rk. NetPacker is based on NetCompressor and dnLib projects.
  
###  Example
I create Intro01 projet for an example that use SQL and OpenGL.
It's for my tests only, so we need a true tiny intro02 OpenGL skeleton without SDL (With Winform ?) ...

### usage

##### Command

netpacker.exe ..\..\..\Examples\Intro01\bin\Release\Intro01.exe ..\..\..\Examples\Intro01\bin\Release\Packed.exe

##### Output
```
NetPacker v0.1
ILOptimizer: Original Size: 69632 bytes

ILOptimizer: Total number of classes: 176
ILOptimizer: Total number of methods: 424
ILOptimizer: Used Methods: 313/424
ILOptimizer: Used Classes: 150/176
ILOptimizer: Removed methods count: 111
ILOptimizer: Packed Size: 63488 bytes

NetPacker: GZip Compressed Size: 26536  Percent: 62
```

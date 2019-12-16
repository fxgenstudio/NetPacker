using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NetPacker
{

    /// <summary>
    /// Method usage description
    /// </summary>
    public class MethodUsage
    {
        public int Count;
        public MethodDef def;
    }

    /// <summary>
    /// Class usage description
    /// </summary>
    public class ClassUsage
    {
        public int Count;
        public TypeDef def;
    }

    /// <summary>
    /// This class remove methods not referenced (TODO classes and fields)
    /// </summary>
    public class ILOptimizer
    {
        Dictionary<string, ClassUsage> m_classesUsage = new Dictionary<string, ClassUsage>();
        Dictionary<string, MethodUsage> m_methodsUsage = new Dictionary<string, MethodUsage>();


        public void Dump(string _strExeFileName)
        {
            var mod = ModuleDefMD.Load(_strExeFileName);

            int totalNumTypes = 0;

            foreach (var type in mod.GetTypes())
            {
                totalNumTypes++;
                Console.WriteLine();
                Console.WriteLine("ILOptimizer: Type: {0}", type.FullName);
                if (!(type.BaseType is null))
                    Console.WriteLine("ILOptimizer:   Base type: {0}", type.BaseType.FullName);

                Console.WriteLine("ILOptimizer:   Methods: {0}", type.Methods.Count);
                Console.WriteLine("ILOptimizer:   Fields: {0}", type.Fields.Count);
                Console.WriteLine("ILOptimizer:   Properties: {0}", type.Properties.Count);
                Console.WriteLine("ILOptimizer:   Events: {0}", type.Events.Count);
                Console.WriteLine("ILOptimizer:   Nested types: {0}", type.NestedTypes.Count);



                if (type.Interfaces.Count > 0)
                {
                    Console.WriteLine("ILOptimizer:   Interfaces:");
                    foreach (var iface in type.Interfaces)
                        Console.WriteLine("ILOptimizer:     {0}", iface.Interface.FullName);
                }
            }
            Console.WriteLine();
            Console.WriteLine("ILOptimizer: Total number of types: {0}", totalNumTypes);
        }


        public void Pack(string _strExeFileName, string _strExePacket)
        {
            //Display current file size
            long length1 = new System.IO.FileInfo(_strExeFileName).Length;
            Console.WriteLine($"ILOptimizer: Original Size: {length1 } bytes");

            // Load Assembly
            var mod = ModuleDefMD.Load(_strExeFileName);

            //Reducing size...
            CreateTypesEntriesForUsagesCount(mod);

            ComputeMethodsUsage(mod);
            ComputeClassesUsage(mod);

            DisplayReports();

            CleanUpMethods(mod);
            //CleanUpClasses(mod); //Doesn't  work yet need remove fields classref

            //public static MakeCurrentDelegate MakeCurrent;              //TODO remove fields !

            //Save new assembly
            mod.Write(_strExePacket);
            mod.Dispose();

            //Display new file size
            long length2 = new System.IO.FileInfo(_strExePacket).Length;
            Console.WriteLine($"ILOptimizer: Packed Size: {length2} bytes");


            Console.WriteLine("");

        }


        void CreateTypesEntriesForUsagesCount(ModuleDefMD mod)
        {


            ///////////////////////////////////////////////////////////////
            // Make methods and classes list

            int totalNumTypes = 0;

            foreach (var type in mod.GetTypes())
            {
                totalNumTypes++;

                //Console.WriteLine("  Methods: {0}", type.Methods.Count);
                //Console.WriteLine("  Fields: {0}", type.Fields.Count);
                //Console.WriteLine("  Properties: {0}", type.Properties.Count);
                //Console.WriteLine("  Events: {0}", type.Events.Count);
                //Console.WriteLine("  Nested types: {0}", type.NestedTypes.Count);

                //if (type.Interfaces.Count > 0)
                //{
                //    Console.WriteLine("  Interfaces:");
                //    foreach (var iface in type.Interfaces)
                //        Console.WriteLine("    {0}", iface.Interface.FullName);
                //}


                //Create class usage entry
                if (type.IsClass)
                {
                    m_classesUsage.Add(type.FullName, new ClassUsage() { def = type });
                }

                //Create method usage entry
                foreach (var m in type.Methods)
                {
                    //Not constructor
                    if (m.IsConstructor == true)
                        continue;

                    m_methodsUsage.Add(m.FullName, new MethodUsage() { def = m });

                    //Default add entry Main Entry point...
                    if (m.Name == mod.EntryPoint.Name)
                    {
                        MethodUsage usage;
                        m_methodsUsage.TryGetValue(m.FullName, out usage);
                        usage.Count++;
                    }
                }



            }
            Console.WriteLine();
            Console.WriteLine("ILOptimizer: Total number of classes: {0}", m_classesUsage.Count);
            Console.WriteLine("ILOptimizer: Total number of methods: {0}", m_methodsUsage.Count);


        }

        void DisplayReports()
        {
            //Display methods usages
            int usedMethods = 0;
            foreach (var mu in m_methodsUsage)
            {
                //Debug.WriteLine($"Method: {mu.Key}  Usage:{mu.Value.Count}" );

                if (mu.Value.Count > 0)
                {
                    usedMethods++;
                }
                else
                {
                    Debug.WriteLine($"ILOptimizer: Method: {mu.Key}  Not used !");
                }
            }

            Console.WriteLine($"ILOptimizer: Used Methods: {usedMethods}/{m_methodsUsage.Count}");


            //Display classes usages
            int usedClasses = 0;
            foreach (var mu in m_classesUsage)
            {
                //Debug.WriteLine($"Method: {mu.Key}  Usage:{mu.Value.Count}" );

                if (mu.Value.Count > 0)
                {
                    usedClasses++;
                }
                else
                {
                    Debug.WriteLine($"Class: {mu.Key}  Not used !");
                }
            }

            Console.WriteLine($"ILOptimizer: Used Classes: {usedClasses}/{m_classesUsage.Count}");
        }

        void ComputeMethodsUsage(ModuleDefMD mod)
        {
            ///////////////////////////////////////////////////////////////
            // Compute methods usages in Methods
            foreach (var type in mod.GetTypes())
            {
                //Research methods usages in methods IL body
                foreach (var m in type.Methods)
                {
                    ProcessTypeMethod(m);
                }
            }
        }

        void ComputeClassesUsage(ModuleDefMD mod)
        {
            foreach (var type in mod.GetTypes())
            {
                ProcessClasses(type);
            }
        }

        void ProcessClasses(TypeDef _td)
        {
            foreach (var m in _td.Methods)
            {

                if (m.HasBody)
                {

                    //Class Methods
                    var calls = m.Body.Instructions.Where(i =>
                        i.OpCode.Code == dnlib.DotNet.Emit.Code.Newobj
                        || i.OpCode.Code == dnlib.DotNet.Emit.Code.Call
                        || i.OpCode.Code == dnlib.DotNet.Emit.Code.Calli
                        || i.OpCode.Code == dnlib.DotNet.Emit.Code.Callvirt
                        || i.OpCode.Code == dnlib.DotNet.Emit.Code.Stsfld   //Delegate

                    );

                    foreach (var c in calls)
                    {


                        if (c.Operand is MethodSpec)
                        {
                            var methodSpec = c.Operand as MethodSpec;


                            ClassUsage usage;
                            if (m_classesUsage.TryGetValue(methodSpec.DeclaringType.FullName, out usage))
                            {
                                //class is used !
                                usage.Count++;
                            }

                        }

                        else if (c.Operand is MethodDef)
                        {
                            var methodDef = c.Operand as MethodDef;


                            ClassUsage usage;
                            if (m_classesUsage.TryGetValue(methodDef.DeclaringType.FullName, out usage))
                            {
                                //class is used !
                                usage.Count++;
                            }

                        }

                        else if (c.Operand is FieldDef)
                        {
                            var fd = c.Operand as FieldDef;

                            var sig = fd.FieldType as ClassSig;
                            if (sig != null)
                            {
                                var td = sig.TypeDef;

                                foreach (var m2 in td.Methods)
                                {
                                    //Class usage
                                    ClassUsage classUsage;
                                    if (m_classesUsage.TryGetValue(m2.DeclaringType.FullName, out classUsage))
                                    {
                                        classUsage.Count++;
                                    }

                                }
                            }

                        }
                    }
                }
            }


            foreach (var f in _td.Fields)
            {

                ClassUsage usage;
                if (m_classesUsage.TryGetValue(f.DeclaringType.FullName, out usage))
                {
                    //class is used !
                    usage.Count++;
                }
            }


            foreach (var a in _td.CustomAttributes)
            {

                ClassUsage usage;
                if (m_classesUsage.TryGetValue(a.TypeFullName, out usage))
                {
                    //class is used !
                    usage.Count++;
                }

            }

        }

        void ProcessTypeMethod(MethodDef m)
        {
            if (m.HasBody)
            {

                //Class Methods
                var calls = m.Body.Instructions.Where(i =>
                    i.OpCode.Code == dnlib.DotNet.Emit.Code.Call
                    || i.OpCode.Code == dnlib.DotNet.Emit.Code.Callvirt
                    || i.OpCode.Code == dnlib.DotNet.Emit.Code.Stsfld   //Delegate
                );

                //var callsVirt = m.Body.Instructions.Where(i => i.OpCode.Code == dnlib.DotNet.Emit.Code.Callvirt);
                //Debug.Assert(callsVirt.Count() == 0);

                var callsI = m.Body.Instructions.Where(i => i.OpCode.Code == dnlib.DotNet.Emit.Code.Calli);
                Debug.Assert(callsI.Count() == 0);



                foreach (var c in calls)
                {
                    MethodDef methodDef = null;

                    //Methods
                    if (c.Operand is MethodSpec)
                    {
                        var methodSpec = c.Operand as MethodSpec;
                        methodDef = methodSpec.Method as MethodDef;
                    }
                    else if (c.Operand is MethodDef)
                    {
                        methodDef = c.Operand as MethodDef;
                    }
                    else if (c.Operand is dnlib.DotNet.FieldDef)
                    {
                        var fd = c.Operand as dnlib.DotNet.FieldDef;



                        var sig = fd.FieldType as ClassSig;
                        if (sig != null)
                        {
                            var td = sig.TypeDef;

                            foreach (var m2 in td.Methods)
                            {
                                //Method usage
                                MethodUsage usage;
                                if (m_methodsUsage.TryGetValue(m2.FullName, out usage))
                                {
                                    //Method is used !
                                    usage.Count++;
                                }

                                //Class usage
                                //ClassUsage classUsage;
                                //if (m_classesUsage.TryGetValue(m2.DeclaringType.FullName, out classUsage))
                                //{
                                //    classUsage.Count++;
                                //}

                                //Debug.WriteLine($"ILOptimizer: Method Not found {m2.FullName} ");

                            }
                        }




                    }

                    if (methodDef == null) continue;

                    //If ILCode => Call class Method
                    if (methodDef != null)
                    {

                        //Method usage
                        MethodUsage usage;
                        if (m_methodsUsage.TryGetValue(methodDef.FullName, out usage))
                        {
                            //Method is used !
                            usage.Count++;

                            //if (methodDef.FullName.Contains("MakeCurrent"))
                            //{
                            //    int a = 1;
                            //}
                        }
                        else
                        {
                            Debug.WriteLine($"ILOptimizer: Method Not found {methodDef.FullName} ");
                        }


                        //Class usage
                        //ClassUsage classUsage;
                        //if (m_classesUsage.TryGetValue(methodDef.DeclaringType.FullName, out classUsage))
                        //{
                        //    classUsage.Count++;
                        //}


                    }
                }

            }

        }

        void CleanUpMethods(ModuleDefMD mod)
        {
            //Remove unless method
            int methodsRemoved = 0;
            foreach (var m in m_methodsUsage)
            {
                var md = m.Value.def;
                if (md == null)
                    continue;

                //if (md.FullName.Contains("GL"))   //Filter
                //    continue;

                if (m.Value.Count == 0 && md != null)
                {
                    //Control if method is part of declaring class
                    var found = md.DeclaringType.Methods.Where(c => c == md).FirstOrDefault();
                    Debug.Assert(found != null);

                    //Remove method from class
                    //Debug.WriteLine($"ILOptimizer: Removing method {md.FullName}");

                    md.DeclaringType.Methods.Remove(md);
                    md.Body = null;

                    methodsRemoved++;
                }
            }


            Console.WriteLine($"ILOptimizer: Removed methods count: {methodsRemoved}");

        }

        void CleanUpClasses(ModuleDefMD mod)
        {
            //Remove unless method
            int classesRemoved = 0;
            foreach (var m in m_classesUsage)
            {
                var md = m.Value.def;
                if (md == null)
                    continue;

                //if (md.FullName.Contains("GL"))   //Filter
                //    continue;

                if (m.Value.Count == 0 && md != null)
                {
                    //Remove method from class
                    Debug.WriteLine($"ILOptimizer: Removing class {md.FullName}");

                    if (md.DeclaringType != null)
                    {
                        //bool bRemoved = md.DeclaringType.Types.Remove(md);
                        bool bRemoved = md.DeclaringType.NestedTypes.Remove(md);

                    }
                    else
                    {
                        //bool bRemoved = md.Module.Types.Remove(md);
                        //if (bRemoved == false)
                        //{
                        //    Debug.WriteLine($"Error while removing {md.FullName}");


                        //}
                        //else
                        //{
                        //    classesRemoved++;

                        //}

                    }


                }
            }


            Console.WriteLine($"ILOptimizer: Removed classes count: {classesRemoved}");
        }

    }
}

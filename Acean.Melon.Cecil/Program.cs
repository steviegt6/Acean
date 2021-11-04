using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.Cil;

namespace Acean.Melon.Cecil
{
    public static class Program
    {
        public const string AsmGen = "Il2CppAssemblyGenerator.dll";

        public static void Main()
        {
            if (!File.Exists(AsmGen))
                throw new Exception("DLL not found.");

            Console.WriteLine("Found generator DLL.");

            AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(AsmGen);

            TypeDefinition melon = asm.MainModule.GetType(
                "MelonLoader.Il2CppAssemblyGenerator.RemoteAPI"
            ).GetNestedType("DefaultHostInfo").GetNestedType("Melon");

            if (melon is null)
                throw new Exception("Failed to resolve Melon type.");

            Console.WriteLine("Found Melon type.");

            MethodDefinition contact = melon.Methods.Single(x => x.Name == "Contact");

            if (contact is null)
                throw new Exception("Failed to resolve Contact method.");

            Console.WriteLine("Found Contact method.");

            Console.WriteLine("Rewriting Contact method.");

            ILContext il = new ILContext(contact);
            ILCursor c = new ILCursor(il);

            void Match() => c.GotoNext(x => x.OpCode == OpCodes.Ldloc_0);
            void Remove(int range) => c.RemoveRange(range);
            // void Null() => c.Emit(OpCodes.Ldnull);
            void String(string str) => c.Emit(OpCodes.Ldstr, str);

            Match();
            Match();
            Remove(2);
            String(""); // Null();
            Match();
            Remove(2);
            String(""); // Null();

            Console.WriteLine("Rewrote Contact method.");
            Console.WriteLine("Creating new assembly file.");

            asm.Write("ACEAN-" + AsmGen);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }

        public static TypeDefinition GetNestedType(this TypeDefinition self, string fullname)
        {
            if (!self.HasNestedTypes)
                return null;

            Collection<TypeDefinition> nestedTypes = self.NestedTypes;

            return nestedTypes.FirstOrDefault(nestedType => nestedType.TypeFullName() == fullname);
        }

        public static string TypeFullName(this TypeReference self) =>
            string.IsNullOrEmpty(self.Namespace)
                ? self.Name
                : self.Namespace + '.' + self.Name;
    }
}

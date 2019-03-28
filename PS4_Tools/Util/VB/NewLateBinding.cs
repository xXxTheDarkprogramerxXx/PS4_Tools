//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Reflection;

//namespace PS4_Tools.Util.VB
//{
//    /// <summary>This class provides helpers that the Visual Basic compiler uses for late binding calls; it is not meant to be called directly from your code.</summary>
//    /// <filterpriority>1</filterpriority>
//    public sealed class NewLateBinding
//    {
//        // Token: 0x0600042E RID: 1070 RVA: 0x0001AEB4 File Offset: 0x00019EB4
//        private NewLateBinding()
//        {
//        }

//        /// <summary>Executes a late-bound method or function call. This helper method is not meant to be called directly from your code.</summary>
//		/// <returns>An instance of the call object.</returns>
//		/// <param name="Instance">An instance of the call object exposing the property or method.</param>
//		/// <param name="Type">The type of the call object.</param>
//		/// <param name="MemberName">The name of the property or method on the call object.</param>
//		/// <param name="Arguments">An array containing the arguments to be passed to the property or method being called.</param>
//		/// <param name="ArgumentNames">An array of argument names.</param>
//		/// <param name="TypeArguments">An array of argument types; used only for generic calls to pass argument types.</param>
//		/// <param name="CopyBack">An array of Boolean values that the late binder uses to communicate back to the call site which arguments match ByRef parameters. Each True value indicates that the arguments matched and should be copied out after the call to LateCall is complete.</param>
//		/// <param name="IgnoreReturn">A Boolean value indicating whether or not the return value can be ignored.</param>
//		/// <PermissionSet>
//		///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
//		/// </PermissionSet>
//        public static object LateCall(object Instance, Type Type, string MemberName, object[] Arguments, string[] ArgumentNames, Type[] TypeArguments, bool[] CopyBack, bool IgnoreReturn)
//        {
//            if (Arguments == null)
//            {
//                Arguments = Symbols.NoArguments;
//            }
//            if (ArgumentNames == null)
//            {
//                ArgumentNames = Symbols.NoArgumentNames;
//            }
//            if (TypeArguments == null)
//            {
//                TypeArguments = Symbols.NoTypeArguments;
//            }
//            Symbols.Container container;
//            if (Type != null)
//            {
//                container = new Symbols.Container(Type);
//            }
//            else
//            {
//                container = new Symbols.Container(Instance);
//            }
//            if (container.IsCOMObject)
//            {
//                return LateBinding.InternalLateCall(Instance, Type, MemberName, Arguments, ArgumentNames, CopyBack, IgnoreReturn);
//            }
//            BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.GetProperty;
//            if (IgnoreReturn)
//            {
//                bindingFlags |= BindingFlags.IgnoreReturn;
//            }
//            OverloadResolution.ResolutionFailure resolutionFailure;
//            return NewLateBinding.CallMethod(container, MemberName, Arguments, ArgumentNames, TypeArguments, CopyBack, bindingFlags, true, ref resolutionFailure);
//        }

//    }
//}

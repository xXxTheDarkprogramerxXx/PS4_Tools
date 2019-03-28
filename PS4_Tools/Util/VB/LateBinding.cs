//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Runtime.Remoting;
//using System.Threading;

//namespace PS4_Tools.Util.VB
//{
//    /// <summary>This class has been deprecated since Visual Basic 2005. </summary>
//	/// <filterpriority>1</filterpriority>
//    public sealed class LateBinding
//    {
//        private LateBinding()
//        {
//        }

//        private static IReflect GetCorrectIReflect(object o, Type objType)
//        {
//            if (o != null && !objType.IsCOMObject && !RemotingServices.IsTransparentProxy(o) && !(o is Type))
//            {
//                IReflect reflect = o as IReflect;
//                if (reflect != null)
//                {
//                    return reflect;
//                }
//            }
//            return objType;
//        }

//        private static void CheckForClassExtendingCOMClass(Type objType)
//        {
//            if (!objType.IsCOMObject)
//            {
//                return;
//            }
//            //if (Operators.CompareString(objType.FullName, "System.__ComObject", false) == 0)
//            //{
//            //    return;
//            //}
//            //if (Operators.CompareString(objType.BaseType.FullName, "System.__ComObject", false) == 0)
//            //{
//            //    return;
//            //}
//            //throw new InvalidOperationException(Utils.GetResourceString("LateboundCallToInheritedComClass"));
//        }


//        internal static object InternalLateCall(object o, Type objType, string name, object[] args, string[] paramnames, bool[] CopyBack, bool IgnoreReturn)
//        {
//            BindingFlags bindingFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.OptionalParamBinding;
//            if (IgnoreReturn)
//            {
//                bindingFlags |= BindingFlags.IgnoreReturn;
//            }
//            if (objType == null)
//            {
//                if (o == null)
//                {
//                    throw new Exception("Object not set to an instance of an object");
//                }
//                objType = o.GetType();
//            }
//            IReflect correctIReflect = LateBinding.GetCorrectIReflect(o, objType);
//            if (objType.IsCOMObject)
//            {
//                LateBinding.CheckForClassExtendingCOMClass(objType);
//            }
//            if (name == null)
//            {
//                name = "";
//            }
//            VBBinder vbbinder = new VBBinder(CopyBack);
//            if (!objType.IsCOMObject)
//            {
//                MemberInfo[] membersByName = LateBinding.GetMembersByName(correctIReflect, name, bindingFlags);
//                if (membersByName == null || membersByName.Length == 0)
//                {
//                    throw new MissingMemberException(Utils.GetResourceString("MissingMember_MemberNotFoundOnType2", new string[]
//                    {
//                        name,
//                        Utils.VBFriendlyName(objType, o)
//                    }));
//                }
//                if (LateBinding.MemberIsField(membersByName))
//                {
//                    throw new ArgumentException(Utils.GetResourceString("ExpressionNotProcedure", new string[]
//                    {
//                        name,
//                        Utils.VBFriendlyName(objType, o)
//                    }));
//                }
//                if (membersByName.Length == 1 && (paramnames == null || paramnames.Length == 0))
//                {
//                    MemberInfo memberInfo = membersByName[0];
//                    if (memberInfo.MemberType == MemberTypes.Property)
//                    {
//                        memberInfo = ((PropertyInfo)memberInfo).GetGetMethod();
//                        if (memberInfo == null)
//                        {
//                            throw new MissingMemberException(Utils.GetResourceString("MissingMember_MemberNotFoundOnType2", new string[]
//                            {
//                                name,
//                                Utils.VBFriendlyName(objType, o)
//                            }));
//                        }
//                    }
//                    MethodBase methodBase = (MethodBase)memberInfo;
//                    ParameterInfo[] parameters = methodBase.GetParameters();
//                    int num = args.Length;
//                    int num2 = parameters.Length;
//                    if (num2 == num)
//                    {
//                        if (num2 == 0)
//                        {
//                            return LateBinding.FastCall(o, methodBase, parameters, args, objType, correctIReflect);
//                        }
//                        if (CopyBack == null && LateBinding.NoByrefs(parameters))
//                        {
//                            ParameterInfo parameterInfo = parameters[checked(num2 - 1)];
//                            if (!parameterInfo.ParameterType.IsArray)
//                            {
//                                return LateBinding.FastCall(o, methodBase, parameters, args, objType, correctIReflect);
//                            }
//                            object[] customAttributes = parameterInfo.GetCustomAttributes(typeof(ParamArrayAttribute), false);
//                            if (customAttributes == null || customAttributes.Length == 0)
//                            {
//                                return LateBinding.FastCall(o, methodBase, parameters, args, objType, correctIReflect);
//                            }
//                        }
//                    }
//                }
//            }
//            object result;
//            try
//            {
//                result = vbbinder.InvokeMember(name, bindingFlags, objType, correctIReflect, o, args, paramnames);
//            }
//            catch (MissingMemberException ex)
//            {
//                throw;
//            }
//            catch (Exception ex2) when (LateBinding.IsMissingMemberException(ex2))
//            {
//                throw new MissingMemberException(Utils.GetResourceString("MissingMember_MemberNotFoundOnType2", new string[]
//                {
//                    name,
//                    Utils.VBFriendlyName(objType, o)
//                }));
//            }
//            catch (TargetInvocationException ex3)
//            {
//                throw ex3.InnerException;
//            }
//            return result;
//        }

//    }
//}

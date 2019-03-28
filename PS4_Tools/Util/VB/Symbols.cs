//using System;
//using System.Collections;
//using System.Reflection;
//using System.Runtime.Remoting;

//namespace PS4_Tools.Util.VB
//{
//    internal class Symbols
//    {
//        // Token: 0x060005BB RID: 1467 RVA: 0x0002F524 File Offset: 0x0002E524
//        private Symbols()
//        {
//        }

//        // Token: 0x060005BC RID: 1468 RVA: 0x0002F52C File Offset: 0x0002E52C
//        static Symbols()
//        {
//            Symbols.OperatorCLSNames[1] = "op_Explicit";
//            Symbols.OperatorCLSNames[2] = "op_Implicit";
//            Symbols.OperatorCLSNames[3] = "op_True";
//            Symbols.OperatorCLSNames[4] = "op_False";
//            Symbols.OperatorCLSNames[5] = "op_UnaryNegation";
//            Symbols.OperatorCLSNames[6] = "op_OnesComplement";
//            Symbols.OperatorCLSNames[7] = "op_UnaryPlus";
//            Symbols.OperatorCLSNames[8] = "op_Addition";
//            Symbols.OperatorCLSNames[9] = "op_Subtraction";
//            Symbols.OperatorCLSNames[10] = "op_Multiply";
//            Symbols.OperatorCLSNames[11] = "op_Division";
//            Symbols.OperatorCLSNames[12] = "op_Exponent";
//            Symbols.OperatorCLSNames[13] = "op_IntegerDivision";
//            Symbols.OperatorCLSNames[14] = "op_Concatenate";
//            Symbols.OperatorCLSNames[15] = "op_LeftShift";
//            Symbols.OperatorCLSNames[16] = "op_RightShift";
//            Symbols.OperatorCLSNames[17] = "op_Modulus";
//            Symbols.OperatorCLSNames[18] = "op_BitwiseOr";
//            Symbols.OperatorCLSNames[19] = "op_ExclusiveOr";
//            Symbols.OperatorCLSNames[20] = "op_BitwiseAnd";
//            Symbols.OperatorCLSNames[21] = "op_Like";
//            Symbols.OperatorCLSNames[22] = "op_Equality";
//            Symbols.OperatorCLSNames[23] = "op_Inequality";
//            Symbols.OperatorCLSNames[24] = "op_LessThan";
//            Symbols.OperatorCLSNames[25] = "op_LessThanOrEqual";
//            Symbols.OperatorCLSNames[26] = "op_GreaterThanOrEqual";
//            Symbols.OperatorCLSNames[27] = "op_GreaterThan";
//            Symbols.OperatorNames = new string[28];
//            Symbols.OperatorNames[1] = "CType";
//            Symbols.OperatorNames[2] = "CType";
//            Symbols.OperatorNames[3] = "IsTrue";
//            Symbols.OperatorNames[4] = "IsFalse";
//            Symbols.OperatorNames[5] = "-";
//            Symbols.OperatorNames[6] = "Not";
//            Symbols.OperatorNames[7] = "+";
//            Symbols.OperatorNames[8] = "+";
//            Symbols.OperatorNames[9] = "-";
//            Symbols.OperatorNames[10] = "*";
//            Symbols.OperatorNames[11] = "/";
//            Symbols.OperatorNames[12] = "^";
//            Symbols.OperatorNames[13] = "\\";
//            Symbols.OperatorNames[14] = "&";
//            Symbols.OperatorNames[15] = "<<";
//            Symbols.OperatorNames[16] = ">>";
//            Symbols.OperatorNames[17] = "Mod";
//            Symbols.OperatorNames[18] = "Or";
//            Symbols.OperatorNames[19] = "Xor";
//            Symbols.OperatorNames[20] = "And";
//            Symbols.OperatorNames[21] = "Like";
//            Symbols.OperatorNames[22] = "=";
//            Symbols.OperatorNames[23] = "<>";
//            Symbols.OperatorNames[24] = "<";
//            Symbols.OperatorNames[25] = "<=";
//            Symbols.OperatorNames[26] = ">=";
//            Symbols.OperatorNames[27] = ">";
//        }

//        // Token: 0x060005BD RID: 1469 RVA: 0x0002F82C File Offset: 0x0002E82C
//        internal static bool IsUnaryOperator(Symbols.UserDefinedOperator Op)
//        {
//            switch (Op)
//            {
//                case Symbols.UserDefinedOperator.Narrow:
//                case Symbols.UserDefinedOperator.Widen:
//                case Symbols.UserDefinedOperator.IsTrue:
//                case Symbols.UserDefinedOperator.IsFalse:
//                case Symbols.UserDefinedOperator.Negate:
//                case Symbols.UserDefinedOperator.Not:
//                case Symbols.UserDefinedOperator.UnaryPlus:
//                    return true;
//                default:
//                    return false;
//            }
//        }

//        // Token: 0x060005BE RID: 1470 RVA: 0x0002F864 File Offset: 0x0002E864
//        internal static bool IsBinaryOperator(Symbols.UserDefinedOperator Op)
//        {
//            switch (Op)
//            {
//                case Symbols.UserDefinedOperator.Plus:
//                case Symbols.UserDefinedOperator.Minus:
//                case Symbols.UserDefinedOperator.Multiply:
//                case Symbols.UserDefinedOperator.Divide:
//                case Symbols.UserDefinedOperator.Power:
//                case Symbols.UserDefinedOperator.IntegralDivide:
//                case Symbols.UserDefinedOperator.Concatenate:
//                case Symbols.UserDefinedOperator.ShiftLeft:
//                case Symbols.UserDefinedOperator.ShiftRight:
//                case Symbols.UserDefinedOperator.Modulus:
//                case Symbols.UserDefinedOperator.Or:
//                case Symbols.UserDefinedOperator.Xor:
//                case Symbols.UserDefinedOperator.And:
//                case Symbols.UserDefinedOperator.Like:
//                case Symbols.UserDefinedOperator.Equal:
//                case Symbols.UserDefinedOperator.NotEqual:
//                case Symbols.UserDefinedOperator.Less:
//                case Symbols.UserDefinedOperator.LessEqual:
//                case Symbols.UserDefinedOperator.GreaterEqual:
//                case Symbols.UserDefinedOperator.Greater:
//                    return true;
//                default:
//                    return false;
//            }
//        }

//        // Token: 0x060005BF RID: 1471 RVA: 0x0002F8D0 File Offset: 0x0002E8D0
//        internal static bool IsUserDefinedOperator(MethodBase Method)
//        {
//            return Method.IsSpecialName && Method.Name.StartsWith("op_", StringComparison.Ordinal);
//        }

//        // Token: 0x060005C0 RID: 1472 RVA: 0x0002F8FC File Offset: 0x0002E8FC
//        internal static bool IsNarrowingConversionOperator(MethodBase Method)
//        {
//            return Method.IsSpecialName && Method.Name.Equals(Symbols.OperatorCLSNames[1]);
//        }

//        // Token: 0x060005C1 RID: 1473 RVA: 0x0002F928 File Offset: 0x0002E928
//        internal static Symbols.UserDefinedOperator MapToUserDefinedOperator(MethodBase Method)
//        {
//            int num = 1;
//            checked
//            {
//                Symbols.UserDefinedOperator userDefinedOperator;
//                for (;;)
//                {
//                    if (Method.Name.Equals(Symbols.OperatorCLSNames[num]))
//                    {
//                        int num2 = Method.GetParameters().Length;
//                        userDefinedOperator = (Symbols.UserDefinedOperator)num;
//                        if (num2 == 1 && Symbols.IsUnaryOperator(userDefinedOperator))
//                        {
//                            break;
//                        }
//                        if (num2 == 2 && Symbols.IsBinaryOperator(userDefinedOperator))
//                        {
//                            break;
//                        }
//                    }
//                    num++;
//                    if (num > 27)
//                    {
//                        return Symbols.UserDefinedOperator.UNDEF;
//                    }
//                }
//                return userDefinedOperator;
//            }
//        }

//        // Token: 0x060005C2 RID: 1474 RVA: 0x0002F980 File Offset: 0x0002E980
//        internal static TypeCode GetTypeCode(Type Type)
//        {
//            return Type.GetTypeCode(Type);
//        }

//        // Token: 0x060005C3 RID: 1475 RVA: 0x0002F994 File Offset: 0x0002E994
//        internal static Type MapTypeCodeToType(TypeCode TypeCode)
//        {
//            switch (TypeCode)
//            {
//                case TypeCode.Object:
//                    return typeof(object);
//                case TypeCode.DBNull:
//                    return typeof(DBNull);
//                case TypeCode.Boolean:
//                    return typeof(bool);
//                case TypeCode.Char:
//                    return typeof(char);
//                case TypeCode.SByte:
//                    return typeof(sbyte);
//                case TypeCode.Byte:
//                    return typeof(byte);
//                case TypeCode.Int16:
//                    return typeof(short);
//                case TypeCode.UInt16:
//                    return typeof(ushort);
//                case TypeCode.Int32:
//                    return typeof(int);
//                case TypeCode.UInt32:
//                    return typeof(uint);
//                case TypeCode.Int64:
//                    return typeof(long);
//                case TypeCode.UInt64:
//                    return typeof(ulong);
//                case TypeCode.Single:
//                    return typeof(float);
//                case TypeCode.Double:
//                    return typeof(double);
//                case TypeCode.Decimal:
//                    return typeof(decimal);
//                case TypeCode.DateTime:
//                    return typeof(DateTime);
//                case TypeCode.String:
//                    return typeof(string);
//            }
//            return null;
//        }

//        // Token: 0x060005C4 RID: 1476 RVA: 0x0002FAB4 File Offset: 0x0002EAB4
//        internal static bool IsRootObjectType(Type Type)
//        {
//            return Type == typeof(object);
//        }

//        // Token: 0x060005C5 RID: 1477 RVA: 0x0002FAD0 File Offset: 0x0002EAD0
//        internal static bool IsRootEnumType(Type Type)
//        {
//            return Type == typeof(Enum);
//        }

//        // Token: 0x060005C6 RID: 1478 RVA: 0x0002FAEC File Offset: 0x0002EAEC
//        internal static bool IsValueType(Type Type)
//        {
//            return Type.IsValueType;
//        }

//        // Token: 0x060005C7 RID: 1479 RVA: 0x0002FB00 File Offset: 0x0002EB00
//        internal static bool IsEnum(Type Type)
//        {
//            return Type.IsEnum;
//        }

//        // Token: 0x060005C8 RID: 1480 RVA: 0x0002FB14 File Offset: 0x0002EB14
//        internal static bool IsArrayType(Type Type)
//        {
//            return Type.IsArray;
//        }

//        // Token: 0x060005C9 RID: 1481 RVA: 0x0002FB28 File Offset: 0x0002EB28
//        internal static bool IsStringType(Type Type)
//        {
//            return Type == typeof(string);
//        }

//        // Token: 0x060005CA RID: 1482 RVA: 0x0002FB44 File Offset: 0x0002EB44
//        internal static bool IsCharArrayRankOne(Type Type)
//        {
//            return Type == typeof(char[]);
//        }

//        // Token: 0x060005CB RID: 1483 RVA: 0x0002FB60 File Offset: 0x0002EB60
//        internal static bool IsIntegralType(TypeCode TypeCode)
//        {
//            switch (TypeCode)
//            {
//                case TypeCode.SByte:
//                case TypeCode.Byte:
//                case TypeCode.Int16:
//                case TypeCode.UInt16:
//                case TypeCode.Int32:
//                case TypeCode.UInt32:
//                case TypeCode.Int64:
//                case TypeCode.UInt64:
//                    return true;
//            }
//            return false;
//        }

//        // Token: 0x060005CC RID: 1484 RVA: 0x0002FBC4 File Offset: 0x0002EBC4
//        internal static bool IsNumericType(TypeCode TypeCode)
//        {
//            switch (TypeCode)
//            {
//                case TypeCode.SByte:
//                case TypeCode.Byte:
//                case TypeCode.Int16:
//                case TypeCode.UInt16:
//                case TypeCode.Int32:
//                case TypeCode.UInt32:
//                case TypeCode.Int64:
//                case TypeCode.UInt64:
//                case TypeCode.Single:
//                case TypeCode.Double:
//                case TypeCode.Decimal:
//                    return true;
//            }
//            return false;
//        }

//        // Token: 0x060005CD RID: 1485 RVA: 0x0002FC28 File Offset: 0x0002EC28
//        internal static bool IsNumericType(Type Type)
//        {
//            return Symbols.IsNumericType(Symbols.GetTypeCode(Type));
//        }

//        // Token: 0x060005CE RID: 1486 RVA: 0x0002FC40 File Offset: 0x0002EC40
//        internal static bool IsIntrinsicType(TypeCode TypeCode)
//        {
//            switch (TypeCode)
//            {
//                case TypeCode.Boolean:
//                case TypeCode.Char:
//                case TypeCode.SByte:
//                case TypeCode.Byte:
//                case TypeCode.Int16:
//                case TypeCode.UInt16:
//                case TypeCode.Int32:
//                case TypeCode.UInt32:
//                case TypeCode.Int64:
//                case TypeCode.UInt64:
//                case TypeCode.Single:
//                case TypeCode.Double:
//                case TypeCode.Decimal:
//                case TypeCode.DateTime:
//                case TypeCode.String:
//                    return true;
//            }
//            return false;
//        }

//        // Token: 0x060005CF RID: 1487 RVA: 0x0002FCA4 File Offset: 0x0002ECA4
//        internal static bool IsIntrinsicType(Type Type)
//        {
//            return Symbols.IsIntrinsicType(Symbols.GetTypeCode(Type)) && !Symbols.IsEnum(Type);
//        }

//        // Token: 0x060005D0 RID: 1488 RVA: 0x0002FCCC File Offset: 0x0002ECCC
//        internal static bool IsClass(Type Type)
//        {
//            return Type.IsClass || Symbols.IsRootEnumType(Type);
//        }

//        // Token: 0x060005D1 RID: 1489 RVA: 0x0002FCEC File Offset: 0x0002ECEC
//        internal static bool IsClassOrValueType(Type Type)
//        {
//            return Symbols.IsValueType(Type) || Symbols.IsClass(Type);
//        }

//        // Token: 0x060005D2 RID: 1490 RVA: 0x0002FD0C File Offset: 0x0002ED0C
//        internal static bool IsInterface(Type Type)
//        {
//            return Type.IsInterface;
//        }

//        // Token: 0x060005D3 RID: 1491 RVA: 0x0002FD20 File Offset: 0x0002ED20
//        internal static bool IsClassOrInterface(Type Type)
//        {
//            return Symbols.IsClass(Type) || Symbols.IsInterface(Type);
//        }

//        // Token: 0x060005D4 RID: 1492 RVA: 0x0002FD40 File Offset: 0x0002ED40
//        internal static bool IsReferenceType(Type Type)
//        {
//            return Symbols.IsClass(Type) || Symbols.IsInterface(Type);
//        }

//        // Token: 0x060005D5 RID: 1493 RVA: 0x0002FD60 File Offset: 0x0002ED60
//        internal static bool IsGenericParameter(Type Type)
//        {
//            return Type.IsGenericParameter;
//        }

//        // Token: 0x060005D6 RID: 1494 RVA: 0x0002FD74 File Offset: 0x0002ED74
//        internal static bool Implements(Type Implementor, Type Interface)
//        {
//            foreach (Type type in Implementor.GetInterfaces())
//            {
//                if (type == Interface)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        // Token: 0x060005D7 RID: 1495 RVA: 0x0002FDA4 File Offset: 0x0002EDA4
//        internal static bool IsOrInheritsFrom(Type Derived, Type Base)
//        {
//            if (Derived == Base)
//            {
//                return true;
//            }
//            if (Derived.IsGenericParameter)
//            {
//                if (Symbols.IsClass(Base) && (Derived.GenericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) > GenericParameterAttributes.None && Symbols.IsOrInheritsFrom(typeof(ValueType), Base))
//                {
//                    return true;
//                }
//                foreach (Type derived in Derived.GetGenericParameterConstraints())
//                {
//                    if (Symbols.IsOrInheritsFrom(derived, Base))
//                    {
//                        return true;
//                    }
//                }
//            }
//            else if (Symbols.IsInterface(Derived))
//            {
//                if (Symbols.IsInterface(Base))
//                {
//                    foreach (Type type in Derived.GetInterfaces())
//                    {
//                        if (type == Base)
//                        {
//                            return true;
//                        }
//                    }
//                }
//            }
//            else if (Symbols.IsClass(Base) && Symbols.IsClassOrValueType(Derived))
//            {
//                return Derived.IsSubclassOf(Base);
//            }
//            return false;
//        }

//        // Token: 0x060005D8 RID: 1496 RVA: 0x0002FE64 File Offset: 0x0002EE64
//        internal static bool IsGeneric(Type Type)
//        {
//            return Type.IsGenericType;
//        }

//        // Token: 0x060005D9 RID: 1497 RVA: 0x0002FE78 File Offset: 0x0002EE78
//        internal static bool IsInstantiatedGeneric(Type Type)
//        {
//            return Type.IsGenericType && !Type.IsGenericTypeDefinition;
//        }

//        // Token: 0x060005DA RID: 1498 RVA: 0x0002FE98 File Offset: 0x0002EE98
//        internal static bool IsGeneric(MethodBase Method)
//        {
//            return Method.IsGenericMethod;
//        }

//        // Token: 0x060005DB RID: 1499 RVA: 0x0002FEAC File Offset: 0x0002EEAC
//        internal static bool IsGeneric(MemberInfo Member)
//        {
//            MethodBase methodBase = Member as MethodBase;
//            return methodBase != null && Symbols.IsGeneric(methodBase);
//        }

//        // Token: 0x060005DC RID: 1500 RVA: 0x0002FECC File Offset: 0x0002EECC
//        internal static bool IsRawGeneric(MethodBase Method)
//        {
//            return Method.IsGenericMethod && Method.IsGenericMethodDefinition;
//        }

//        // Token: 0x060005DD RID: 1501 RVA: 0x0002FEEC File Offset: 0x0002EEEC
//        internal static Type[] GetTypeParameters(MemberInfo Member)
//        {
//            MethodBase methodBase = Member as MethodBase;
//            if (methodBase == null)
//            {
//                return Symbols.NoTypeParameters;
//            }
//            return methodBase.GetGenericArguments();
//        }

//        // Token: 0x060005DE RID: 1502 RVA: 0x0002FF10 File Offset: 0x0002EF10
//        internal static Type[] GetTypeParameters(Type Type)
//        {
//            return Type.GetGenericArguments();
//        }

//        // Token: 0x060005DF RID: 1503 RVA: 0x0002FF24 File Offset: 0x0002EF24
//        internal static Type[] GetTypeArguments(Type Type)
//        {
//            return Type.GetGenericArguments();
//        }

//        // Token: 0x060005E0 RID: 1504 RVA: 0x0002FF38 File Offset: 0x0002EF38
//        internal static Type[] GetInterfaceConstraints(Type GenericParameter)
//        {
//            return GenericParameter.GetInterfaces();
//        }

//        // Token: 0x060005E1 RID: 1505 RVA: 0x0002FF4C File Offset: 0x0002EF4C
//        internal static Type GetClassConstraint(Type GenericParameter)
//        {
//            Type baseType = GenericParameter.BaseType;
//            if (Symbols.IsRootObjectType(baseType))
//            {
//                return null;
//            }
//            return baseType;
//        }

//        // Token: 0x060005E2 RID: 1506 RVA: 0x0002FF6C File Offset: 0x0002EF6C
//        internal static int IndexIn(Type PossibleGenericParameter, MethodBase GenericMethodDef)
//        {
//            if (Symbols.IsGenericParameter(PossibleGenericParameter) && PossibleGenericParameter.DeclaringMethod != null && Symbols.AreGenericMethodDefsEqual(PossibleGenericParameter.DeclaringMethod, GenericMethodDef))
//            {
//                return PossibleGenericParameter.GenericParameterPosition;
//            }
//            return -1;
//        }

//        // Token: 0x060005E3 RID: 1507 RVA: 0x0002FFA0 File Offset: 0x0002EFA0
//        internal static bool RefersToGenericParameter(Type ReferringType, MethodBase Method)
//        {
//            if (!Symbols.IsRawGeneric(Method))
//            {
//                return false;
//            }
//            if (ReferringType.IsByRef)
//            {
//                ReferringType = Symbols.GetElementType(ReferringType);
//            }
//            if (Symbols.IsGenericParameter(ReferringType))
//            {
//                if (Symbols.AreGenericMethodDefsEqual(ReferringType.DeclaringMethod, Method))
//                {
//                    return true;
//                }
//            }
//            else if (Symbols.IsGeneric(ReferringType))
//            {
//                foreach (Type referringType in Symbols.GetTypeArguments(ReferringType))
//                {
//                    if (Symbols.RefersToGenericParameter(referringType, Method))
//                    {
//                        return true;
//                    }
//                }
//            }
//            else if (Symbols.IsArrayType(ReferringType))
//            {
//                return Symbols.RefersToGenericParameter(ReferringType.GetElementType(), Method);
//            }
//            return false;
//        }

//        // Token: 0x060005E4 RID: 1508 RVA: 0x00030024 File Offset: 0x0002F024
//        internal static bool RefersToGenericParameterCLRSemantics(Type ReferringType, Type Typ)
//        {
//            if (ReferringType.IsByRef)
//            {
//                ReferringType = Symbols.GetElementType(ReferringType);
//            }
//            if (Symbols.IsGenericParameter(ReferringType))
//            {
//                if (ReferringType.DeclaringType == Typ)
//                {
//                    return true;
//                }
//            }
//            else if (Symbols.IsGeneric(ReferringType))
//            {
//                foreach (Type referringType in Symbols.GetTypeArguments(ReferringType))
//                {
//                    if (Symbols.RefersToGenericParameterCLRSemantics(referringType, Typ))
//                    {
//                        return true;
//                    }
//                }
//            }
//            else if (Symbols.IsArrayType(ReferringType))
//            {
//                return Symbols.RefersToGenericParameterCLRSemantics(ReferringType.GetElementType(), Typ);
//            }
//            return false;
//        }

//        // Token: 0x060005E5 RID: 1509 RVA: 0x00030098 File Offset: 0x0002F098
//        internal static bool AreGenericMethodDefsEqual(MethodBase Method1, MethodBase Method2)
//        {
//            return Method1 == Method2 || Method1.MetadataToken == Method2.MetadataToken;
//        }

//        // Token: 0x060005E6 RID: 1510 RVA: 0x000300BC File Offset: 0x0002F0BC
//        internal static bool IsShadows(MethodBase Method)
//        {
//            return !Method.IsHideBySig && (!Method.IsVirtual || (Method.Attributes & MethodAttributes.VtableLayoutMask) != MethodAttributes.PrivateScope || (((MethodInfo)Method).GetBaseDefinition().Attributes & MethodAttributes.VtableLayoutMask) != MethodAttributes.PrivateScope);
//        }

//        // Token: 0x060005E7 RID: 1511 RVA: 0x00030108 File Offset: 0x0002F108
//        internal static bool IsShared(MemberInfo Member)
//        {
//            switch (Member.MemberType)
//            {
//                case MemberTypes.Constructor:
//                    return ((ConstructorInfo)Member).IsStatic;
//                case MemberTypes.Field:
//                    return ((FieldInfo)Member).IsStatic;
//                case MemberTypes.Method:
//                    return ((MethodInfo)Member).IsStatic;
//                case MemberTypes.Property:
//                    return ((PropertyInfo)Member).GetGetMethod().IsStatic;
//            }
//            return false;
//        }

//        // Token: 0x060005E8 RID: 1512 RVA: 0x0003019C File Offset: 0x0002F19C
//        internal static bool IsParamArray(ParameterInfo Parameter)
//        {
//            return Symbols.IsArrayType(Parameter.ParameterType) && Parameter.IsDefined(typeof(ParamArrayAttribute), false);
//        }

//        // Token: 0x060005E9 RID: 1513 RVA: 0x000301CC File Offset: 0x0002F1CC
//        internal static Type GetElementType(Type Type)
//        {
//            return Type.GetElementType();
//        }

//        // Token: 0x060005EA RID: 1514 RVA: 0x000301E0 File Offset: 0x0002F1E0
//        internal static bool AreParametersAndReturnTypesValid(ParameterInfo[] Parameters, Type ReturnType)
//        {
//            if (ReturnType != null && (ReturnType.IsPointer || ReturnType.IsByRef))
//            {
//                return false;
//            }
//            if (Parameters != null)
//            {
//                foreach (ParameterInfo parameterInfo in Parameters)
//                {
//                    if (parameterInfo.ParameterType.IsPointer)
//                    {
//                        return false;
//                    }
//                }
//            }
//            return true;
//        }

//        // Token: 0x060005EB RID: 1515 RVA: 0x0003022C File Offset: 0x0002F22C
//        internal static void GetAllParameterCounts(ParameterInfo[] Parameters, ref int RequiredParameterCount, ref int MaximumParameterCount, ref int ParamArrayIndex)
//        {
//            MaximumParameterCount = Parameters.Length;
//            checked
//            {
//                for (int i = MaximumParameterCount - 1; i >= 0; i += -1)
//                {
//                    if (!Parameters[i].IsOptional)
//                    {
//                        RequiredParameterCount = i + 1;
//                        break;
//                    }
//                }
//                if (MaximumParameterCount != 0 && Symbols.IsParamArray(Parameters[MaximumParameterCount - 1]))
//                {
//                    ParamArrayIndex = MaximumParameterCount - 1;
//                    RequiredParameterCount--;
//                }
//            }
//        }

//        // Token: 0x060005EC RID: 1516 RVA: 0x0003027C File Offset: 0x0002F27C
//        internal static bool IsNonPublicRuntimeMember(MemberInfo Member)
//        {
//            Type declaringType = Member.DeclaringType;
//            return !declaringType.IsPublic && declaringType.Assembly == Utils.VBRuntimeAssembly;
//        }

//        // Token: 0x060005ED RID: 1517 RVA: 0x000302A8 File Offset: 0x0002F2A8
//        internal static bool HasFlag(BindingFlags Flags, BindingFlags FlagToTest)
//        {
//            return (Flags & FlagToTest) > BindingFlags.Default;
//        }

//        // Token: 0x0400044D RID: 1101
//        internal static readonly object[] NoArguments = new object[0];

//        // Token: 0x0400044E RID: 1102
//        internal static readonly string[] NoArgumentNames = new string[0];

//        // Token: 0x0400044F RID: 1103
//        internal static readonly Type[] NoTypeArguments = new Type[0];

//        // Token: 0x04000450 RID: 1104
//        internal static readonly Type[] NoTypeParameters = new Type[0];

//        // Token: 0x04000451 RID: 1105
//        internal static readonly string[] OperatorCLSNames = new string[28];

//        // Token: 0x04000452 RID: 1106
//        internal static readonly string[] OperatorNames;

//        // Token: 0x020000AF RID: 175
//        internal enum UserDefinedOperator : sbyte
//        {
//            // Token: 0x04000454 RID: 1108
//            UNDEF,
//            // Token: 0x04000455 RID: 1109
//            Narrow,
//            // Token: 0x04000456 RID: 1110
//            Widen,
//            // Token: 0x04000457 RID: 1111
//            IsTrue,
//            // Token: 0x04000458 RID: 1112
//            IsFalse,
//            // Token: 0x04000459 RID: 1113
//            Negate,
//            // Token: 0x0400045A RID: 1114
//            Not,
//            // Token: 0x0400045B RID: 1115
//            UnaryPlus,
//            // Token: 0x0400045C RID: 1116
//            Plus,
//            // Token: 0x0400045D RID: 1117
//            Minus,
//            // Token: 0x0400045E RID: 1118
//            Multiply,
//            // Token: 0x0400045F RID: 1119
//            Divide,
//            // Token: 0x04000460 RID: 1120
//            Power,
//            // Token: 0x04000461 RID: 1121
//            IntegralDivide,
//            // Token: 0x04000462 RID: 1122
//            Concatenate,
//            // Token: 0x04000463 RID: 1123
//            ShiftLeft,
//            // Token: 0x04000464 RID: 1124
//            ShiftRight,
//            // Token: 0x04000465 RID: 1125
//            Modulus,
//            // Token: 0x04000466 RID: 1126
//            Or,
//            // Token: 0x04000467 RID: 1127
//            Xor,
//            // Token: 0x04000468 RID: 1128
//            And,
//            // Token: 0x04000469 RID: 1129
//            Like,
//            // Token: 0x0400046A RID: 1130
//            Equal,
//            // Token: 0x0400046B RID: 1131
//            NotEqual,
//            // Token: 0x0400046C RID: 1132
//            Less,
//            // Token: 0x0400046D RID: 1133
//            LessEqual,
//            // Token: 0x0400046E RID: 1134
//            GreaterEqual,
//            // Token: 0x0400046F RID: 1135
//            Greater,
//            // Token: 0x04000470 RID: 1136
//            MAX
//        }

//        // Token: 0x020000B0 RID: 176
//        internal sealed class Container
//        {
//            // Token: 0x060005EF RID: 1519 RVA: 0x000302CC File Offset: 0x0002F2CC
//            internal Container(object Instance)
//            {
//                if (Instance == null)
//                {
//                    throw ExceptionUtils.VbMakeException(91);
//                }
//                this.m_Instance = Instance;
//                this.m_Type = Instance.GetType();
//                this.m_UseCustomReflection = false;
//                if (!this.m_Type.IsCOMObject && !RemotingServices.IsTransparentProxy(Instance) && !(Instance is Type))
//                {
//                    this.m_IReflect = (Instance as IReflect);
//                    if (this.m_IReflect != null)
//                    {
//                        this.m_UseCustomReflection = true;
//                    }
//                }
//                if (!this.m_UseCustomReflection)
//                {
//                    this.m_IReflect = this.m_Type;
//                }
//                this.CheckForClassExtendingCOMClass();
//            }

//            // Token: 0x060005F0 RID: 1520 RVA: 0x00030358 File Offset: 0x0002F358
//            internal Container(Type Type)
//            {
//                if (Type == null)
//                {
//                    throw ExceptionUtils.VbMakeException(91);
//                }
//                this.m_Instance = null;
//                this.m_Type = Type;
//                this.m_IReflect = Type;
//                this.m_UseCustomReflection = false;
//                this.CheckForClassExtendingCOMClass();
//            }

//            // Token: 0x170000AD RID: 173
//            // (get) Token: 0x060005F1 RID: 1521 RVA: 0x00030390 File Offset: 0x0002F390
//            internal bool IsCOMObject
//            {
//                get
//                {
//                    return this.m_Type.IsCOMObject;
//                }
//            }

//            // Token: 0x170000AE RID: 174
//            // (get) Token: 0x060005F2 RID: 1522 RVA: 0x000303A8 File Offset: 0x0002F3A8
//            internal string VBFriendlyName
//            {
//                get
//                {
//                    return Utils.VBFriendlyName(this.m_Type, this.m_Instance);
//                }
//            }

//            // Token: 0x170000AF RID: 175
//            // (get) Token: 0x060005F3 RID: 1523 RVA: 0x000303C8 File Offset: 0x0002F3C8
//            internal bool IsArray
//            {
//                get
//                {
//                    return Symbols.IsArrayType(this.m_Type) && this.m_Instance != null;
//                }
//            }

//            // Token: 0x170000B0 RID: 176
//            // (get) Token: 0x060005F4 RID: 1524 RVA: 0x000303F0 File Offset: 0x0002F3F0
//            internal bool IsValueType
//            {
//                get
//                {
//                    return Symbols.IsValueType(this.m_Type) && this.m_Instance != null;
//                }
//            }

//            // Token: 0x060005F5 RID: 1525 RVA: 0x00030418 File Offset: 0x0002F418
//            private static MemberInfo[] FilterInvalidMembers(MemberInfo[] Members)
//            {
//                if (Members == null || Members.Length == 0)
//                {
//                    return null;
//                }
//                int num = 0;
//                int num2 = 0;
//                checked
//                {
//                    int num3 = Members.Length - 1;
//                    for (int i = num2; i <= num3; i++)
//                    {
//                        ParameterInfo[] array = null;
//                        Type returnType = null;
//                        switch (Members[i].MemberType)
//                        {
//                            case MemberTypes.Constructor:
//                            case MemberTypes.Method:
//                                {
//                                    MethodInfo methodInfo = (MethodInfo)Members[i];
//                                    array = methodInfo.GetParameters();
//                                    returnType = methodInfo.ReturnType;
//                                    break;
//                                }
//                            case MemberTypes.Field:
//                                returnType = ((FieldInfo)Members[i]).FieldType;
//                                break;
//                            case MemberTypes.Property:
//                                {
//                                    PropertyInfo propertyInfo = (PropertyInfo)Members[i];
//                                    MethodInfo getMethod = propertyInfo.GetGetMethod();
//                                    if (getMethod != null)
//                                    {
//                                        array = getMethod.GetParameters();
//                                    }
//                                    else
//                                    {
//                                        MethodInfo setMethod = propertyInfo.GetSetMethod();
//                                        ParameterInfo[] parameters = setMethod.GetParameters();
//                                        array = new ParameterInfo[parameters.Length - 2 + 1];
//                                        Array.Copy(parameters, array, array.Length);
//                                    }
//                                    returnType = propertyInfo.PropertyType;
//                                    break;
//                                }
//                        }
//                        if (Symbols.AreParametersAndReturnTypesValid(array, returnType))
//                        {
//                            num++;
//                        }
//                        else
//                        {
//                            Members[i] = null;
//                        }
//                    }
//                    if (num == Members.Length)
//                    {
//                        return Members;
//                    }
//                    if (num > 0)
//                    {
//                        MemberInfo[] array2 = new MemberInfo[num - 1 + 1];
//                        int num4 = 0;
//                        int num5 = 0;
//                        int num6 = Members.Length - 1;
//                        for (int i = num5; i <= num6; i++)
//                        {
//                            if (Members[i] != null)
//                            {
//                                array2[num4] = Members[i];
//                                num4++;
//                            }
//                        }
//                        return array2;
//                    }
//                    return null;
//                }
//            }

//            // Token: 0x060005F6 RID: 1526 RVA: 0x00030584 File Offset: 0x0002F584
//            internal MemberInfo[] LookupNamedMembers(string MemberName)
//            {
//                MemberInfo[] array;
//                if (Symbols.IsGenericParameter(this.m_Type))
//                {
//                    Type classConstraint = Symbols.GetClassConstraint(this.m_Type);
//                    if (classConstraint != null)
//                    {
//                        array = classConstraint.GetMember(MemberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
//                    }
//                    else
//                    {
//                        array = null;
//                    }
//                }
//                else
//                {
//                    array = this.m_IReflect.GetMember(MemberName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
//                }
//                array = Symbols.Container.FilterInvalidMembers(array);
//                if (array == null)
//                {
//                    array = Symbols.Container.NoMembers;
//                }
//                else if (array.Length > 1)
//                {
//                    Array.Sort(array, Symbols.Container.InheritanceSorter.Instance);
//                }
//                return array;
//            }

//            // Token: 0x060005F7 RID: 1527 RVA: 0x000305F0 File Offset: 0x0002F5F0
//            private MemberInfo[] LookupDefaultMembers(ref string DefaultMemberName)
//            {
//                string text = null;
//                Type type = this.m_Type;
//                object[] customAttributes;
//                for (;;)
//                {
//                    customAttributes = type.GetCustomAttributes(typeof(DefaultMemberAttribute), false);
//                    if (customAttributes != null && customAttributes.Length > 0)
//                    {
//                        break;
//                    }
//                    type = type.BaseType;
//                    if (type == null || Symbols.IsRootObjectType(type))
//                    {
//                        goto IL_46;
//                    }
//                }
//                text = ((DefaultMemberAttribute)customAttributes[0]).MemberName;
//                IL_46:
//                if (text != null)
//                {
//                    MemberInfo[] array = type.GetMember(text, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
//                    array = Symbols.Container.FilterInvalidMembers(array);
//                    if (array != null)
//                    {
//                        DefaultMemberName = text;
//                        if (array.Length > 1)
//                        {
//                            Array.Sort(array, Symbols.Container.InheritanceSorter.Instance);
//                        }
//                        return array;
//                    }
//                }
//                return Symbols.Container.NoMembers;
//            }

//            // Token: 0x060005F8 RID: 1528 RVA: 0x0003067C File Offset: 0x0002F67C
//            internal MemberInfo[] GetMembers(ref string MemberName, bool ReportErrors)
//            {
//                if (MemberName == null)
//                {
//                    MemberName = "";
//                }
//                MemberInfo[] array;
//                if (Operators.CompareString(MemberName, "", false) == 0)
//                {
//                    if (this.m_UseCustomReflection)
//                    {
//                        array = this.LookupNamedMembers(MemberName);
//                    }
//                    else
//                    {
//                        array = this.LookupDefaultMembers(ref MemberName);
//                    }
//                    if (array.Length == 0)
//                    {
//                        if (ReportErrors)
//                        {
//                            throw new MissingMemberException(Utils.GetResourceString("MissingMember_NoDefaultMemberFound1", new string[]
//                            {
//                                this.VBFriendlyName
//                            }));
//                        }
//                        return array;
//                    }
//                    else if (this.m_UseCustomReflection)
//                    {
//                        MemberName = array[0].Name;
//                    }
//                }
//                else
//                {
//                    array = this.LookupNamedMembers(MemberName);
//                    if (array.Length == 0)
//                    {
//                        if (ReportErrors)
//                        {
//                            throw new MissingMemberException(Utils.GetResourceString("MissingMember_MemberNotFoundOnType2", new string[]
//                            {
//                                MemberName,
//                                this.VBFriendlyName
//                            }));
//                        }
//                        return array;
//                    }
//                }
//                return array;
//            }

//            // Token: 0x060005F9 RID: 1529 RVA: 0x0003073C File Offset: 0x0002F73C
//            private void CheckForClassExtendingCOMClass()
//            {
//                if (this.IsCOMObject && Operators.CompareString(this.m_Type.FullName, "System.__ComObject", false) != 0 && Operators.CompareString(this.m_Type.BaseType.FullName, "System.__ComObject", false) != 0)
//                {
//                    throw new InvalidOperationException(Utils.GetResourceString("LateboundCallToInheritedComClass"));
//                }
//            }

//            // Token: 0x060005FA RID: 1530 RVA: 0x00030798 File Offset: 0x0002F798
//            internal object GetFieldValue(FieldInfo Field)
//            {
//                if (this.m_Instance == null && !Symbols.IsShared(Field))
//                {
//                    throw new NullReferenceException(Utils.GetResourceString("NullReference_InstanceReqToAccessMember1", new string[]
//                    {
//                        Utils.FieldToString(Field)
//                    }));
//                }
//                if (Symbols.IsNonPublicRuntimeMember(Field))
//                {
//                    throw new MissingMemberException();
//                }
//                return Field.GetValue(this.m_Instance);
//            }

//            // Token: 0x060005FB RID: 1531 RVA: 0x000307F0 File Offset: 0x0002F7F0
//            internal void SetFieldValue(FieldInfo Field, object Value)
//            {
//                if (Field.IsInitOnly)
//                {
//                    throw new MissingMemberException(Utils.GetResourceString("MissingMember_ReadOnlyField2", new string[]
//                    {
//                        Field.Name,
//                        this.VBFriendlyName
//                    }));
//                }
//                if (this.m_Instance == null && !Symbols.IsShared(Field))
//                {
//                    throw new NullReferenceException(Utils.GetResourceString("NullReference_InstanceReqToAccessMember1", new string[]
//                    {
//                        Utils.FieldToString(Field)
//                    }));
//                }
//                if (Symbols.IsNonPublicRuntimeMember(Field))
//                {
//                    throw new MissingMemberException();
//                }
//                Field.SetValue(this.m_Instance, Conversions.ChangeType(Value, Field.FieldType));
//            }

//            // Token: 0x060005FC RID: 1532 RVA: 0x00030888 File Offset: 0x0002F888
//            internal object GetArrayValue(object[] Indices)
//            {
//                Array array = (Array)this.m_Instance;
//                int rank = array.Rank;
//                if (Indices.Length != rank)
//                {
//                    throw new RankException();
//                }
//                int num = (int)Conversions.ChangeType(Indices[0], typeof(int));
//                if (rank == 1)
//                {
//                    return array.GetValue(num);
//                }
//                int num2 = (int)Conversions.ChangeType(Indices[1], typeof(int));
//                if (rank == 2)
//                {
//                    return array.GetValue(num, num2);
//                }
//                int num3 = (int)Conversions.ChangeType(Indices[2], typeof(int));
//                if (rank == 3)
//                {
//                    return array.GetValue(num, num2, num3);
//                }
//                checked
//                {
//                    int[] array2 = new int[rank - 1 + 1];
//                    array2[0] = num;
//                    array2[1] = num2;
//                    array2[2] = num3;
//                    int num4 = 3;
//                    int num5 = rank - 1;
//                    for (int i = num4; i <= num5; i++)
//                    {
//                        array2[i] = (int)Conversions.ChangeType(Indices[i], typeof(int));
//                    }
//                    return array.GetValue(array2);
//                }
//            }

//            // Token: 0x060005FD RID: 1533 RVA: 0x00030994 File Offset: 0x0002F994
//            internal void SetArrayValue(object[] Arguments)
//            {
//                Array array = (Array)this.m_Instance;
//                int rank = array.Rank;
//                checked
//                {
//                    if (Arguments.Length - 1 != rank)
//                    {
//                        throw new RankException();
//                    }
//                    object expression = Arguments[Arguments.Length - 1];
//                    Type elementType = this.m_Type.GetElementType();
//                    int num = (int)Conversions.ChangeType(Arguments[0], typeof(int));
//                    if (rank == 1)
//                    {
//                        array.SetValue(Conversions.ChangeType(expression, elementType), num);
//                        return;
//                    }
//                    int num2 = (int)Conversions.ChangeType(Arguments[1], typeof(int));
//                    if (rank == 2)
//                    {
//                        array.SetValue(Conversions.ChangeType(expression, elementType), num, num2);
//                        return;
//                    }
//                    int num3 = (int)Conversions.ChangeType(Arguments[2], typeof(int));
//                    if (rank == 3)
//                    {
//                        array.SetValue(Conversions.ChangeType(expression, elementType), num, num2, num3);
//                        return;
//                    }
//                    int[] array2 = new int[rank - 1 + 1];
//                    array2[0] = num;
//                    array2[1] = num2;
//                    array2[2] = num3;
//                    int num4 = 3;
//                    int num5 = rank - 1;
//                    for (int i = num4; i <= num5; i++)
//                    {
//                        array2[i] = (int)Conversions.ChangeType(Arguments[i], typeof(int));
//                    }
//                    array.SetValue(Conversions.ChangeType(expression, elementType), array2);
//                }
//            }

//            // Token: 0x060005FE RID: 1534 RVA: 0x00030AD8 File Offset: 0x0002FAD8
//            internal object InvokeMethod(Symbols.Method TargetProcedure, object[] Arguments, bool[] CopyBack, BindingFlags Flags)
//            {
//                MethodBase callTarget = NewLateBinding.GetCallTarget(TargetProcedure, Flags);
//                object[] array = NewLateBinding.ConstructCallArguments(TargetProcedure, Arguments, Flags);
//                if (this.m_Instance == null && !Symbols.IsShared(callTarget))
//                {
//                    throw new NullReferenceException(Utils.GetResourceString("NullReference_InstanceReqToAccessMember1", new string[]
//                    {
//                        TargetProcedure.ToString()
//                    }));
//                }
//                if (Symbols.IsNonPublicRuntimeMember(callTarget))
//                {
//                    throw new MissingMemberException();
//                }
//                object result;
//                try
//                {
//                    result = callTarget.Invoke(this.m_Instance, array);
//                }
//                catch (TargetInvocationException ex) when (ex.InnerException != null)
//                {
//                    throw ex.InnerException;
//                }
//                OverloadResolution.ReorderArgumentArray(TargetProcedure, array, Arguments, CopyBack, Flags);
//                return result;
//            }

//            // Token: 0x04000471 RID: 1137
//            private readonly object m_Instance;

//            // Token: 0x04000472 RID: 1138
//            private readonly Type m_Type;

//            // Token: 0x04000473 RID: 1139
//            private readonly IReflect m_IReflect;

//            // Token: 0x04000474 RID: 1140
//            private readonly bool m_UseCustomReflection;

//            // Token: 0x04000475 RID: 1141
//            private const BindingFlags DefaultLookupFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

//            // Token: 0x04000476 RID: 1142
//            private static readonly MemberInfo[] NoMembers = new MemberInfo[0];

//            // Token: 0x020000B3 RID: 179
//            private class InheritanceSorter : IComparer
//            {
//                // Token: 0x06000617 RID: 1559 RVA: 0x00030B9C File Offset: 0x0002FB9C
//                private InheritanceSorter()
//                {
//                }

//                // Token: 0x06000618 RID: 1560 RVA: 0x00030BA4 File Offset: 0x0002FBA4
//                int IComparer.Compare(object Left, object Right)
//                {
//                    Type declaringType = ((MemberInfo)Left).DeclaringType;
//                    Type declaringType2 = ((MemberInfo)Right).DeclaringType;
//                    if (declaringType == declaringType2)
//                    {
//                        return 0;
//                    }
//                    if (declaringType.IsSubclassOf(declaringType2))
//                    {
//                        return -1;
//                    }
//                    return 1;
//                }

//                // Token: 0x04000488 RID: 1160
//                internal static readonly Symbols.Container.InheritanceSorter Instance = new Symbols.Container.InheritanceSorter();
//            }
//        }

//        // Token: 0x020000B1 RID: 177
//        internal sealed class Method
//        {
//            // Token: 0x060005FF RID: 1535 RVA: 0x00030BDC File Offset: 0x0002FBDC
//            private Method(ParameterInfo[] Parameters, int ParamArrayIndex, bool ParamArrayExpanded)
//            {
//                this.m_Parameters = Parameters;
//                this.m_RawParameters = Parameters;
//                this.ParamArrayIndex = ParamArrayIndex;
//                this.ParamArrayExpanded = ParamArrayExpanded;
//                this.AllNarrowingIsFromObject = true;
//            }

//            // Token: 0x06000600 RID: 1536 RVA: 0x00030C08 File Offset: 0x0002FC08
//            internal Method(MethodBase Method, ParameterInfo[] Parameters, int ParamArrayIndex, bool ParamArrayExpanded) : this(Parameters, ParamArrayIndex, ParamArrayExpanded)
//            {
//                this.m_Item = Method;
//                this.m_RawItem = Method;
//            }

//            // Token: 0x06000601 RID: 1537 RVA: 0x00030C24 File Offset: 0x0002FC24
//            internal Method(PropertyInfo Property, ParameterInfo[] Parameters, int ParamArrayIndex, bool ParamArrayExpanded) : this(Parameters, ParamArrayIndex, ParamArrayExpanded)
//            {
//                this.m_Item = Property;
//            }

//            // Token: 0x170000B1 RID: 177
//            // (get) Token: 0x06000602 RID: 1538 RVA: 0x00030C38 File Offset: 0x0002FC38
//            internal ParameterInfo[] Parameters
//            {
//                get
//                {
//                    return this.m_Parameters;
//                }
//            }

//            // Token: 0x170000B2 RID: 178
//            // (get) Token: 0x06000603 RID: 1539 RVA: 0x00030C4C File Offset: 0x0002FC4C
//            internal ParameterInfo[] RawParameters
//            {
//                get
//                {
//                    return this.m_RawParameters;
//                }
//            }

//            // Token: 0x170000B3 RID: 179
//            // (get) Token: 0x06000604 RID: 1540 RVA: 0x00030C60 File Offset: 0x0002FC60
//            internal ParameterInfo[] RawParametersFromType
//            {
//                get
//                {
//                    if (this.m_RawParametersFromType == null)
//                    {
//                        if (!this.IsProperty)
//                        {
//                            int metadataToken = this.m_Item.MetadataToken;
//                            Type declaringType = this.m_Item.DeclaringType;
//                            MethodBase methodBase = declaringType.Module.ResolveMethod(metadataToken, null, null);
//                            this.m_RawParametersFromType = methodBase.GetParameters();
//                        }
//                        else
//                        {
//                            this.m_RawParametersFromType = this.m_RawParameters;
//                        }
//                    }
//                    return this.m_RawParametersFromType;
//                }
//            }

//            // Token: 0x170000B4 RID: 180
//            // (get) Token: 0x06000605 RID: 1541 RVA: 0x00030CC4 File Offset: 0x0002FCC4
//            internal Type DeclaringType
//            {
//                get
//                {
//                    return this.m_Item.DeclaringType;
//                }
//            }

//            // Token: 0x170000B5 RID: 181
//            // (get) Token: 0x06000606 RID: 1542 RVA: 0x00030CDC File Offset: 0x0002FCDC
//            internal Type RawDeclaringType
//            {
//                get
//                {
//                    if (this.m_RawDeclaringType == null)
//                    {
//                        Type declaringType = this.m_Item.DeclaringType;
//                        int metadataToken = declaringType.MetadataToken;
//                        this.m_RawDeclaringType = declaringType.Module.ResolveType(metadataToken, null, null);
//                    }
//                    return this.m_RawDeclaringType;
//                }
//            }

//            // Token: 0x170000B6 RID: 182
//            // (get) Token: 0x06000607 RID: 1543 RVA: 0x00030D20 File Offset: 0x0002FD20
//            internal bool HasParamArray
//            {
//                get
//                {
//                    return this.ParamArrayIndex > -1;
//                }
//            }

//            // Token: 0x170000B7 RID: 183
//            // (get) Token: 0x06000608 RID: 1544 RVA: 0x00030D38 File Offset: 0x0002FD38
//            internal bool HasByRefParameter
//            {
//                get
//                {
//                    foreach (ParameterInfo parameterInfo in this.Parameters)
//                    {
//                        if (parameterInfo.ParameterType.IsByRef)
//                        {
//                            return true;
//                        }
//                    }
//                    return false;
//                }
//            }

//            // Token: 0x170000B8 RID: 184
//            // (get) Token: 0x06000609 RID: 1545 RVA: 0x00030D70 File Offset: 0x0002FD70
//            internal bool IsProperty
//            {
//                get
//                {
//                    return this.m_Item.MemberType == MemberTypes.Property;
//                }
//            }

//            // Token: 0x170000B9 RID: 185
//            // (get) Token: 0x0600060A RID: 1546 RVA: 0x00030D8C File Offset: 0x0002FD8C
//            internal bool IsMethod
//            {
//                get
//                {
//                    return this.m_Item.MemberType == MemberTypes.Method || this.m_Item.MemberType == MemberTypes.Constructor;
//                }
//            }

//            // Token: 0x170000BA RID: 186
//            // (get) Token: 0x0600060B RID: 1547 RVA: 0x00030DB8 File Offset: 0x0002FDB8
//            internal bool IsGeneric
//            {
//                get
//                {
//                    return Symbols.IsGeneric(this.m_Item);
//                }
//            }

//            // Token: 0x170000BB RID: 187
//            // (get) Token: 0x0600060C RID: 1548 RVA: 0x00030DD0 File Offset: 0x0002FDD0
//            internal Type[] TypeParameters
//            {
//                get
//                {
//                    return Symbols.GetTypeParameters(this.m_Item);
//                }
//            }

//            // Token: 0x0600060D RID: 1549 RVA: 0x00030DE8 File Offset: 0x0002FDE8
//            internal bool BindGenericArguments()
//            {
//                bool result;
//                try
//                {
//                    this.m_Item = ((MethodInfo)this.m_RawItem).MakeGenericMethod(this.TypeArguments);
//                    this.m_Parameters = this.AsMethod().GetParameters();
//                    result = true;
//                }
//                catch (ArgumentException ex)
//                {
//                    result = false;
//                }
//                return result;
//            }

//            // Token: 0x0600060E RID: 1550 RVA: 0x00030E3C File Offset: 0x0002FE3C
//            internal MethodBase AsMethod()
//            {
//                return this.m_Item as MethodBase;
//            }

//            // Token: 0x0600060F RID: 1551 RVA: 0x00030E54 File Offset: 0x0002FE54
//            internal PropertyInfo AsProperty()
//            {
//                return this.m_Item as PropertyInfo;
//            }

//            // Token: 0x06000610 RID: 1552 RVA: 0x00030E6C File Offset: 0x0002FE6C
//            public static bool operator ==(Symbols.Method Left, Symbols.Method Right)
//            {
//                return Left.m_Item == Right.m_Item;
//            }

//            // Token: 0x06000611 RID: 1553 RVA: 0x00030E88 File Offset: 0x0002FE88
//            public static bool operator !=(Symbols.Method Left, Symbols.Method right)
//            {
//                return Left.m_Item != right.m_Item;
//            }

//            // Token: 0x06000612 RID: 1554 RVA: 0x00030EA8 File Offset: 0x0002FEA8
//            public static bool operator ==(MemberInfo Left, Symbols.Method Right)
//            {
//                return Left == Right.m_Item;
//            }

//            // Token: 0x06000613 RID: 1555 RVA: 0x00030EC0 File Offset: 0x0002FEC0
//            public static bool operator !=(MemberInfo Left, Symbols.Method Right)
//            {
//                return Left != Right.m_Item;
//            }

//            // Token: 0x06000614 RID: 1556 RVA: 0x00030EDC File Offset: 0x0002FEDC
//            public override string ToString()
//            {
//                return Utils.MemberToString(this.m_Item);
//            }

//            // Token: 0x04000477 RID: 1143
//            private MemberInfo m_Item;

//            // Token: 0x04000478 RID: 1144
//            private MethodBase m_RawItem;

//            // Token: 0x04000479 RID: 1145
//            private ParameterInfo[] m_Parameters;

//            // Token: 0x0400047A RID: 1146
//            private ParameterInfo[] m_RawParameters;

//            // Token: 0x0400047B RID: 1147
//            private ParameterInfo[] m_RawParametersFromType;

//            // Token: 0x0400047C RID: 1148
//            private Type m_RawDeclaringType;

//            // Token: 0x0400047D RID: 1149
//            internal readonly int ParamArrayIndex;

//            // Token: 0x0400047E RID: 1150
//            internal readonly bool ParamArrayExpanded;

//            // Token: 0x0400047F RID: 1151
//            internal bool NotCallable;

//            // Token: 0x04000480 RID: 1152
//            internal bool RequiresNarrowingConversion;

//            // Token: 0x04000481 RID: 1153
//            internal bool AllNarrowingIsFromObject;

//            // Token: 0x04000482 RID: 1154
//            internal bool LessSpecific;

//            // Token: 0x04000483 RID: 1155
//            internal bool ArgumentsValidated;

//            // Token: 0x04000484 RID: 1156
//            internal int[] NamedArgumentMapping;

//            // Token: 0x04000485 RID: 1157
//            internal Type[] TypeArguments;

//            // Token: 0x04000486 RID: 1158
//            internal bool ArgumentMatchingDone;
//        }

//        // Token: 0x020000B2 RID: 178
//        internal sealed class TypedNothing
//        {
//            // Token: 0x06000615 RID: 1557 RVA: 0x00030EF4 File Offset: 0x0002FEF4
//            internal TypedNothing(Type Type)
//            {
//                this.Type = Type;
//            }

//            // Token: 0x04000487 RID: 1159
//            internal readonly Type Type;
//        }
//    }
//}

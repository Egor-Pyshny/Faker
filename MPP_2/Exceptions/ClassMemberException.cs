using System.Reflection;

namespace MPP_2.Exceptions
{
    public class ClassMemberException : Exception
    {
        public ClassMemberException(Type classType, MemberInfo member) : base($"Class {classType.FullName} dont have field or property {member.Name}") { }
    }
}

namespace Tmp.Repository;

using Tmp.Interface.Repository;

public sealed class UserRepository(TmpContext context) : DatabaseRepository(context), IUserRepository
{
}

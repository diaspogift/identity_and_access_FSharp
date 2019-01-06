namespace IdentityAndAccess.DatabaseFunctionsInterfaceTypes


open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.DomainTypes

 








   
    
    
module RoleDb =



///Role related types
/// 
/// 
/// 
    type SaveOneRole = Role -> Result<unit, string>

    type LoadOneRoleById =  RoleId -> Result<Role, string> 

    type UpdateOneRole = Role -> Result<unit, string> 







///User related types
/// 
/// 
    type SaveOneUser = User -> Result<unit, string>

    type LoadOneUserById = UserId -> Result<User, string> 

    type UpdateOneUser = User -> Result<string, string> 





///Tenant related types
/// 
/// 
    type SaveOneTenant = Tenant -> Result<unit, string>

    type LoadOneTenantById = TenantId -> Result<Tenant, string> 

    type UpdateOneTenant = Tenant -> Result<string, string> 






///Group related types
/// 
/// 
    type SaveOneGroup = Group -> unit

    type LoadOneGroupById = GroupId -> Result<Group, string> 
    
    type LoadOneGroupByGroupMemberId = GroupMemberId -> Result<Group, string> 

    type UpdateOneGroup = Group -> unit



        









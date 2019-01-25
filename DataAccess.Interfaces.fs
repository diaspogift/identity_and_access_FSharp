namespace IdentityAndAccess.DatabaseFunctionsInterfaceTypes


open IdentityAndAcccess.CommonDomainTypes
open IdentityAndAcccess.DomainTypes
open IdentityAndAcccess.DomainTypes.Role
open IdentityAndAcccess.DomainTypes.User
open IdentityAndAcccess.DomainTypes.Group
open IdentityAndAcccess.DomainTypes.Tenant
open IdentityAndAcccess.DomainTypes.Functions

 








   
    
    
module RoleDb =



    ///Role related types
    /// 
    /// 
    /// 
    /// 
    type SaveOneRole = Role.Role -> Result<unit, string>

    type LoadOneRoleById =  RoleId -> Result<Dto.Role, string> 

    type LoadOneRoleByRoleIdAndTenantId =   RoleId -> TenantId -> Result<Dto.Role, string> 

    type UpdateOneRole = Role.Role -> Result<unit, string> 





module UserDb =








    ///User related types
    /// 
    /// 
    /// 
    /// 
    type SaveOneUser = 

        User.User -> Result<unit, string>






    type LoadOneUserById = 

        UserId -> Result<Dto.User, string> 






    type LoadOneUserByUserIdAndTenantId = 
         
         UserId -> TenantId -> Result<Dto.User, string>






    type UpdateOneUser = 
        
        User.User -> Result<unit, string> 






module TenantDb =





    ///Tenant related types
    /// 
    /// 
    /// 
    /// 
    type SaveOneTenant = 
        
        Tenant.Tenant -> Result<unit, string>





    type LoadOneTenantById = 
    
        TenantId -> Result<Tenant.Tenant, string> 






    type UpdateOneTenant = 
    
        Tenant.Tenant -> Result<unit, string> 



















module GroupDb =










    ///Group related types
    /// 
    /// 
    /// 
    ///  
    type SaveOneGroup = 
        Group.Group-> Result<unit, string>



    type LoadOneGroupById = 
        GroupId -> Result<Group.Group, string> 



    type LoadOneGroupByGroupIdAndTenantId =  
        GroupId -> TenantId -> Result<Group.Group, string>


    
    type LoadOneGroupByGroupMemberId = 
        GroupMemberId -> Result<Group.Group, string> 



    type LoadOneGroupByGroupMemberIdAndTenantId =  
       GroupMemberId -> TenantId -> Result<Group.Group, string>




    type UpdateOneGroup = 
       Group.Group -> Result<unit,string>



        









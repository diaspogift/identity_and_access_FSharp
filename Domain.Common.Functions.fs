module public IdentityAndAcccess.CommonDomainTypes.Functions

open System
open IdentityAndAcccess.CommonDomainTypes




type FieldNameCreationMessage = FieldNameCreationMessage of string 






module ConstrainedType =



    let createString fieldName ctor maxLen str = 
            
        if String.IsNullOrEmpty(str) then
            let msg = sprintf "%s must not be null or empty" fieldName 
            Error msg
        elif str.Length > maxLen then
            let msg = sprintf "%s must not be more than %i chars" fieldName maxLen 
            Error msg 
        else
            let ok = Ok (ctor str)
            ok





    let createStringControlledLength fieldName ctor minLen maxLen str = 
            
        if String.IsNullOrEmpty(str) then
            let msg = sprintf "%s must not be null or empty" fieldName 
            Error msg
        elif str.Length > maxLen then
            let msg = sprintf "%s must not be more than %i chars" fieldName maxLen 
            Error msg 
        elif str.Length < minLen then
            let msg = sprintf "%s must not be less than %i chars" fieldName minLen 
            Error msg 
        else
            let ok = Ok (ctor str)
            ok





    let createStringOption fieldName ctor maxLen str = 
        if String.IsNullOrEmpty(str) then
            Ok None
        elif str.Length > maxLen then
            let msg = sprintf "%s must not be more than %i chars" fieldName maxLen 
            Error msg 
        else
            Ok (ctor str |> Some)

       




    let createInt fieldName ctor minVal maxVal i = 
        if i < minVal then
            let msg = sprintf "%s: Must not be less than %i" fieldName minVal
            Error msg
        elif i > maxVal then
            let msg = sprintf "%s: Must not be greater than %i" fieldName maxVal
            Error msg
        else
            Ok (ctor i)




        
    let createDecimal fieldName ctor minVal maxVal i = 
        if i < minVal then
            let msg = sprintf "%s: Must not be less than %M" fieldName minVal
            Error msg
        elif i > maxVal then
            let msg = sprintf "%s: Must not be greater than %M" fieldName maxVal
            Error msg
        else
            Ok (ctor i)




    let createLike fieldName  ctor pattern str = 
        if String.IsNullOrEmpty(str) then
            let msg = sprintf "%s: Must not be null or empty" fieldName 
            Error msg
        elif System.Text.RegularExpressions.Regex.IsMatch(str,pattern) then
            Ok (ctor str)
        else
            let msg = sprintf "%s: '%s' must match the pattern '%s'" fieldName str pattern
            Error msg 



    let createDatetimeSpan (fieldName:string)  (startDate:DateTime) (endDate:DateTime) =

        if DateTime.Compare (startDate, endDate) <=  0 then

            let dateTimeSpan:DateTimeSpan  = { Start =  startDate; End =  endDate} 

            Ok { Start =  startDate; End =  endDate}

        else

            let msg = sprintf "%s: Start date must be inferior to end date" fieldName

            Error msg  
           

        
        
        

        
         





module String50 =

    let value (String50 str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName String50 50 str

    let create' = create "50 chars string"

    let createOption fieldName str = 
        ConstrainedType.createStringOption fieldName String50 50 str 
    
    let createOption' = createOption "Optional String"



module RoleId =



    let value (RoleId str) = str

    let create fieldName str = 
            ConstrainedType.createString fieldName RoleId 36 str
    let create' = create   "RoleId : "





module  RoleName =



    let value (RoleName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName RoleName 50 str

    let create' = create  "RoleName : "






module RoleDescription =



    let value (RoleDescription str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName RoleDescription 1000 str

    let create' = create   "RoleDescription : "





module TenantId =


    let value (TenantId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName TenantId 36 str

    let create' = create  "TenantId : "







module  TenantName =


    let value (TenantName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName TenantName 250 str

    let create' = create  "TenantName : "







module TenantDescription =


    let value (TenantDescription str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName TenantDescription 1000 str

    let create' = create   "TenantDescription : "








module UserId =

    let value (UserId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName UserId 36 str

    let create' = create  "UserId :"







module UserDescriptorId =

    let value (UserDescriptorId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName UserDescriptorId 36 str

    let create' = create  "UserId :"






module Username =

    let value (Username str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName Username 50 str

    let create' = create  "Username :"







module Password =

    let value (Password str) = str

    let create fieldName str = 
        ConstrainedType.createStringControlledLength fieldName Password 6 50 str

    let create' = create  "Password :"





module StrongPassword =

    let value (StrongPassword str) = str

    let create fieldName str = 
        ConstrainedType.createStringControlledLength fieldName StrongPassword 6 50 str

    let create' = create  "Strong Password :"










module EncrytedPassword = 


    let value (EncrytedPassword str) = str

    let create fieldName str = 
        ConstrainedType.createStringControlledLength fieldName EncrytedPassword  6 50 str

    let create' = create  "EncrytedPassword :"









module FirstName =

    let value (FirstName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName FirstName 50 str

    let create' = create  "FirstName :"
        





module MiddleName =

    let value (MiddleName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName MiddleName 50 str

    let create' = create  "MiddleName :"







module LastName =

    let value (LastName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName LastName 100 str

    let create' = create  "LastName :"





module RegistrationInvitationId =

    let value (RegistrationInvitationId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName RegistrationInvitationId 36 str

    let create' = create  "RegistrationInvitationId :"






module RegistrationInvitationDescription =

    let value (RegistrationInvitationDescription str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName RegistrationInvitationDescription 1000 str

    let create' = create  "RegistrationInvitationDescription :"






module GroupId =

    let value (GroupId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupId 36 str

    let create' = create  "GroupId :"





module GroupName =

    let value (GroupName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupName 250 str

    let create' = create  "GroupName :"






module GroupDescription =

    let value (GroupDescription str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupDescription 1000 str

    let create' = create  "GroupDescription : "






module GroupMemberId =

    let value (GroupMemberId str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupMemberId 36 str

    let create' = create  "GroupMemberId : "




        


module GroupMemberName =

    let value (GroupMemberName str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName GroupMemberName 150 str
    
    let create' = create  "GroupMemberName : "







module EmailAddress =

    let value (EmailAddress str) = str

    let create fieldName str = 
        let pattern = ".+@.+" 
        ConstrainedType.createLike fieldName EmailAddress pattern str

    let create' = create  "EmailAddress : "






module PostalAddress =

    let value (PostalAddress str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName PostalAddress 1000 str
    
    let create' = create  "PostalAddress : "






module Telephone =

    let value (Telephone str) = str

    let create fieldName str = 
        ConstrainedType.createString fieldName Telephone 50 str

    let create' = create  "Telephone : "






module DateTimeWrapped =

    let value (DateTimeWrapped dateTime) = dateTime

 








[<AutoOpen>]
module ResultComputationExpression =

    type ResultBuilder() =
        member __.Return(x) = Ok x
        member __.Bind(x, f) = Result.bind f x
    
        member __.ReturnFrom(x) = x
        member this.Zero() = this.Return ()

        member __.Delay(f) = f
        member __.Run(f) = f()

        member this.While(guard, body) =
            if not (guard()) 
            then this.Zero() 
            else this.Bind( body(), fun () -> 
                this.While(guard, body))  

        member this.TryWith(body, handler) =
            try this.ReturnFrom(body())
            with e -> handler e

        member this.TryFinally(body, compensation) =
            try this.ReturnFrom(body())
            finally compensation() 

        member this.Using(disposable:#System.IDisposable, body) =
            let body' = fun () -> body disposable
            this.TryFinally(body', fun () -> 
                match disposable with 
                    | null -> () 
                    | disp -> disp.Dispose())

        member this.For(sequence:seq<_>, body) =
            this.Using(sequence.GetEnumerator(),fun enum -> 
                this.While(enum.MoveNext, 
                    this.Delay(fun () -> body enum.Current)))

        member this.Combine (a,b) = 
            this.Bind(a, fun () -> b())


        member  __.Preppend  firstR restR = 
            match firstR, restR with
            | Ok first, Ok rest -> Ok (first::rest)  
            | Error error1, Ok _ -> Error error1
            | Ok _, Error error2 -> Error error2  
            | Error error1, Error _ -> Error error1





        member  __.Sequence aListOfResults =
            let initialValue = Ok List.empty
            List.foldBack  __.Preppend  aListOfResults initialValue


    let result = new ResultBuilder()

 















//==============================================
// Async utilities
//==============================================

[<RequireQualifiedAccess>]  // RequireQualifiedAccess forces the `Async.xxx` prefix to be used
module Async =

    /// Lift a function to Async
    let map f xA = 
        async { 
        let! x = xA
        return f x 
        }

    /// Lift a value to Async
    let retn x = 
        let p = async.Return x
        printfn "IN retn p = %A" p
        p

    /// Apply an Async function to an Async value 
    let apply fA xA = 
        async { 
         // start the two asyncs in parallel
        let! fChild = Async.StartChild fA  // run in parallel
        let! x = xA
        // wait for the result of the first one
        let! f = fChild
        return f x 
        }

    /// Apply a monadic function to an Async value  
    let bind f xA = async.Bind(xA,f)
























//==============================================
// The `Validation` type is the same as the `Result` type but with a *list* for failures
// rather than a single value. This allows `Validation` types to be combined
// by combining their errors ("applicative-style")
//==============================================

type Validation<'Success,'Failure> = 
    Result<'Success,'Failure list>

/// Functions for the `Validation` type (mostly applicative)
[<RequireQualifiedAccess>]  // RequireQualifiedAccess forces the `Validation.xxx` prefix to be used
module Validation =

    /// Apply a Validation<fn> to a Validation<x> applicatively
    let apply (fV:Validation<_,_>) (xV:Validation<_,_>) :Validation<_,_> = 
        match fV, xV with
        | Ok f, Ok x -> Ok (f x)
        | Error errs1, Ok _ -> Error errs1
        | Ok _, Error errs2 -> Error errs2
        | Error errs1, Error errs2 -> Error (errs1 @ errs2)

    // combine a list of Validation, applicatively
    let sequence (aListOfValidations:Validation<_,_> list) = 
        let (<*>) = apply
        let (<!>) = Result.map
        let cons head tail = head::tail
        let consR headR tailR = cons <!> headR <*> tailR
        let initialValue = Ok [] // empty list inside Result
  
        // loop through the list, prepending each element
        // to the initial value
        List.foldBack consR aListOfValidations initialValue

    //-----------------------------------
    // Converting between Validations and other types
    
    let ofResult xR :Validation<_,_> = 
        xR |> Result.mapError List.singleton

    let toResult (xV:Validation<_,_>) :Result<_,_> = 
        xV


//==============================================
// AsyncResult
//==============================================

type AsyncResult<'Success,'Failure> = 
    Async<Result<'Success,'Failure>>

[<RequireQualifiedAccess>]  // RequireQualifiedAccess forces the `AsyncResult.xxx` prefix to be used
module AsyncResult =

    /// Lift a function to AsyncResult
    let map f (x:AsyncResult<_,_>) : AsyncResult<_,_> =
        Async.map (Result.map f) x

    /// Lift a function to AsyncResult
    let mapError f (x:AsyncResult<_,_>) : AsyncResult<_,_> =
        Async.map (Result.mapError f) x

    /// Apply ignore to the internal value
    let ignore x = 
        x |> map ignore    

    /// Lift a value to AsyncResult
    let retn x : AsyncResult<_,_> = 
        x |> Result.Ok |> Async.retn

    /// Handles asynchronous exceptions and maps them into Failure cases using the provided function
    let catch f (x:AsyncResult<_,_>) : AsyncResult<_,_> =
        x
        |> Async.Catch
        |> Async.map(function
            | Choice1Of2 (Ok v) -> Ok v
            | Choice1Of2 (Error err) -> Error err
            | Choice2Of2 ex -> Error (f ex))


    /// Apply an AsyncResult function to an AsyncResult value, monadically
    let applyM (fAsyncResult : AsyncResult<_, _>) (xAsyncResult : AsyncResult<_, _>) :AsyncResult<_,_> = 
        fAsyncResult |> Async.bind (fun fResult ->
        xAsyncResult |> Async.map (fun xResult -> Result.apply fResult xResult))

    /// Apply an AsyncResult function to an AsyncResult value, applicatively
    let applyA (fAsyncResult : AsyncResult<_, _>) (xAsyncResult : AsyncResult<_, _>) :AsyncResult<_,_> = 
        fAsyncResult |> Async.bind (fun fResult ->
        xAsyncResult |> Async.map (fun xResult -> Validation.apply fResult xResult))

    /// Apply a monadic function to an AsyncResult value  
    let bind (f: 'a -> AsyncResult<'b,'c>) (xAsyncResult : AsyncResult<_, _>) :AsyncResult<_,_> = async {
        let! xResult = xAsyncResult 
        match xResult with
        | Ok x -> return! f x
        | Error err -> return (Error err)
        }


    /// Convert a list of AsyncResult into a AsyncResult<list> using monadic style. 
    /// Only the first error is returned. The error type need not be a list.
    let sequenceM resultList = 
        let (<*>) = applyM
        let (<!>) = map
        let cons head tail = head::tail
        let consR headR tailR = cons <!> headR <*> tailR
        let initialValue = retn [] // empty list inside Result
  
        // loop through the list, prepending each element
        // to the initial value
        List.foldBack consR resultList  initialValue


    /// Convert a list of AsyncResult into a AsyncResult<list> using applicative style. 
    /// All the errors are returned. The error type must be a list.
    let sequenceA resultList = 
        let (<*>) = applyA
        let (<!>) = map
        let cons head tail = head::tail
        let consR headR tailR = cons <!> headR <*> tailR
        let initialValue = retn [] // empty list inside Result
  
        // loop through the list, prepending each element
        // to the initial value
        List.foldBack consR resultList  initialValue

    //-----------------------------------
    // Converting between AsyncResults and other types

    /// Lift a value into an Ok inside a AsyncResult
    let ofSuccess x : AsyncResult<_,_> = 
        x |> Result.Ok |> Async.retn 

    /// Lift a value into an Error inside a AsyncResult
    let ofError x : AsyncResult<_,_> = 
        x |> Result.Error |> Async.retn 

    /// Lift a Result into an AsyncResult
    let ofResult (x:Result<_,_>) : AsyncResult<_,_> = 
        printfn "IN ofResult and X = %A" x
        let t = x |> async.Return
        printfn "IN ofResult and t = %A" tanh
        t

    /// Lift a Async into an AsyncResult
    let ofAsync x : AsyncResult<_,_> = 
        x |> Async.map Result.Ok

    //-----------------------------------
    // Utilities lifted from Async

    let sleep ms = 
        Async.Sleep ms |> ofAsync












/// The `asyncResult` computation expression is available globally without qualification
[<AutoOpen>]
module AsyncResultComputationExpression = 

    type AsyncResultBuilder() = 
        member __.Return(x) = AsyncResult.retn x
        member __.Bind(x, f) = AsyncResult.bind f x

        member __.ReturnFrom(x) = x
        member this.Zero() = this.Return ()

        member __.Delay(f) = f
        member __.Run(f) = f()

        member this.While(guard, body) =
            if not (guard()) 
            then this.Zero() 
            else this.Bind( body(), fun () -> 
                this.While(guard, body))  

        member this.TryWith(body, handler) =
            try this.ReturnFrom(body())
            with e -> handler e

        member this.TryFinally(body, compensation) =
            try this.ReturnFrom(body())
            finally compensation() 

        member this.Using(disposable:#System.IDisposable, body) =
            let body' = fun () -> body disposable
            this.TryFinally(body', fun () -> 
                match disposable with 
                    | null -> () 
                    | disp -> disp.Dispose())

        member this.For(sequence:seq<_>, body) =
            this.Using(sequence.GetEnumerator(),fun enum -> 
                this.While(enum.MoveNext, 
                    this.Delay(fun () -> body enum.Current)))

        member this.Combine (a,b) = 
            this.Bind(a, fun () -> b())

    let asyncResult = AsyncResultBuilder()
module Bitcoin
//#load "BitcoinLibs.fs"

open System
open NBitcoin
open NBitcoin.Protocol



// --------------- CREATING A TRANSACTION --------------- // 

let network = 
    // Select your network
    Network.Main
    // Network.Test

/// Generate a new wallet private key for a new user as byte array
let getNewPrivateKey() = Key().ToBytes()

/// Create BitcoinSecret from byte array
let getSecret(bytes:byte[]) = BitcoinSecret(Key(bytes), network)

// Create a new wallet:
let newWallet() = Key()
let alice = BitcoinSecret(newWallet(), network)
let bob = BitcoinSecret(newWallet(), network)
    
// You can also use existing wallets.
// to test you can create wallets e.g. in https://rushwallet.com/
let tuomas = BitcoinSecret("5J3RWPbfuR3W8sQkPto15CBWgc99SiMBNvP7KYb9xdAUW35L2qh", network)

let bobPublic = 
    //bob.GetAddress()
    // or use existing:
    BitcoinPubKeyAddress("1PnfqAxqmH9r5ADc9xCoaXJRjcuFs2xh1H", network)
    
let transaction =
    let sum = Money.Coins 0.008m
    let fee = Money.Coins 0.0004m
    // Previous Transaction-ID, get it from your wallet.
    let previousTransactionId =  uint256("70c3a3925d6a6d652f9a2f4c5b0a93c93c783c9939d701e3a1b24fce6b2244b9")
    // Coin-order-number
    // (Inside a transaction can be many operations, you have to spot your coin.)

    let coin = Coin(OutPoint( previousTransactionId, 0), TxOut(sum, tuomas.ScriptPubKey)) :> ICoin
    let builder = network.CreateTransactionBuilder()
    let tx = 
        builder
            .AddCoins([|coin|])
            .AddKeys(tuomas)
            .Send(bobPublic, (sum - fee))
            .SendFees(fee)
            .SetChange(tuomas.GetAddress())
            .BuildTransaction(true)

    let ok, errs = builder.Verify tx
    match ok with
    | true -> tx
    | false -> failwith(String.Join(", ", errs))

// --------------- SENDING TRANSACTION TO SERVER --------------- // 

// Do the trade
let sendSync() =
    /// Connect to public Bitcoin network:
    // get a bit-node server address, e.g. from: https://bitnodes.21.co/
    // Not all the nodes work.
    use node = Node.Connect(network, "81.17.27.134:8333")
    node.VersionHandshake()
    // Notify server
    node.SendMessage(InvPayload(transaction)) 
    System.Threading.Thread.Sleep 1000
    // Send transaction
    node.SendMessage(TxPayload(transaction))
    System.Threading.Thread.Sleep 5000
    node.Disconnect()
    transaction.GetHash()
//sendSync()

//let sendAsync() =
//    async {
//        use node = Node.Connect(network, "81.17.27.134:8333")
//        node.VersionHandshake()
//        do! node.SendMessageAsync(InvPayload(transaction)) 
//            |> Async.AwaitTask
//        do! Async.Sleep 1000
//        do! node.SendMessageAsync(TxPayload(transaction)) 
//            |> Async.AwaitTask
//        do! Async.Sleep 5000
//        node.DisconnectAsync()
//        Console.WriteLine (transaction.GetHash())
//    }
//sendAsync() |> Async.Start

// Instead of sending, you can broadcast the 
// transaction manually e.g. from http://blockr.io
let transactionToBroadcast = transaction.ToHex()

// You can search the transaction 
// e.g. from: https://blockchain.info/
let hash = transaction.GetHash()

// Some resources:
// Open book: https://programmingblockchain.gitbooks.io/programmingblockchain/content/
// A video: https://www.youtube.com/watch?v=X4ZwRWIF49w
// Article: https://www.codeproject.com/articles/835098/nbitcoin-build-them-all
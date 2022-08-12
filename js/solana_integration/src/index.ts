import { PhantomWalletAdapter } from '@solana/wallet-adapter-phantom';
import { GlowWalletAdapter } from '@solana/wallet-adapter-glow';
import { SlopeWalletAdapter } from '@solana/wallet-adapter-slope';
import { SolflareWalletAdapter } from '@solana/wallet-adapter-solflare';
import { BitKeepWalletAdapter } from '@solana/wallet-adapter-bitkeep';
import { LedgerWalletAdapter } from '@solana/wallet-adapter-ledger';
import { SolletWalletAdapter } from '@solana/wallet-adapter-sollet';
import { BitpieWalletAdapter } from '@solana/wallet-adapter-bitpie';
import { BloctoWalletAdapter } from '@solana/wallet-adapter-blocto';
import { CloverWalletAdapter } from '@solana/wallet-adapter-clover';
import { Coin98WalletAdapter } from '@solana/wallet-adapter-coin98';
import { CoinhubWalletAdapter } from '@solana/wallet-adapter-coinhub';
import { MathWalletAdapter } from '@solana/wallet-adapter-mathwallet';
import { SafePalWalletAdapter } from '@solana/wallet-adapter-safepal';
import { SolongWalletAdapter } from '@solana/wallet-adapter-solong';
import { TokenPocketWalletAdapter } from '@solana/wallet-adapter-tokenpocket';
import { NufiWalletAdapter } from '@solana/wallet-adapter-nufi'
import { AvanaWalletAdapter } from '@solana/wallet-adapter-wallets';
import { BackpackWalletAdapter } from '@solana/wallet-adapter-wallets';
import { BraveWalletAdapter } from '@solana/wallet-adapter-wallets';
import { CoinbaseWalletAdapter } from '@solana/wallet-adapter-wallets';
import { ExodusWalletAdapter } from '@solana/wallet-adapter-wallets';
import { HuobiWalletAdapter } from '@solana/wallet-adapter-wallets';
import { HyperPayWalletAdapter } from '@solana/wallet-adapter-wallets';
import { KeystoneWalletAdapter } from '@solana/wallet-adapter-wallets';
import { NekoWalletAdapter } from '@solana/wallet-adapter-wallets';
import { NightlyWalletAdapter } from '@solana/wallet-adapter-wallets';
import { SaifuWalletAdapter } from '@solana/wallet-adapter-wallets';
import { SalmonWalletAdapter } from '@solana/wallet-adapter-wallets';
import { SkyWalletAdapter } from '@solana/wallet-adapter-wallets';
import { StrikeWalletAdapter } from '@solana/wallet-adapter-wallets';
import { TokenaryWalletAdapter } from '@solana/wallet-adapter-wallets';
import { TrustWalletAdapter } from '@solana/wallet-adapter-wallets';



import {
    WalletNotReadyError,
    WalletReadyState,
} from '@solana/wallet-adapter-base';
import { 
    PublicKey ,
    Transaction
} from '@solana/web3.js';
import { BaseMessageSignerWalletAdapter } from "@solana/wallet-adapter-base";
import { Connection, programs } from '@metaplex/js';


const { metadata: { Metadata } } = programs;
const METADATA_PROGRAM = "metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s";
const METADATA_PREFIX = "metadata";
const metaProgamPrefixBuffer = new TextEncoder().encode(METADATA_PREFIX);
const metaProgamPublicKey = new PublicKey(METADATA_PROGRAM);
const metaProgamPublicKeyBuffer = metaProgamPublicKey.toBuffer();

// switch to devned if nessecary
export const connection = new Connection('mainnet-beta');


let wallets = [
    new PhantomWalletAdapter(),
    new GlowWalletAdapter(),
    new SlopeWalletAdapter(),
    new SolflareWalletAdapter(),
    new BitKeepWalletAdapter(),
    new LedgerWalletAdapter(),
    new SolletWalletAdapter(),
    new BitpieWalletAdapter(),
    new BloctoWalletAdapter(),
    new CloverWalletAdapter(),
    new Coin98WalletAdapter(),
    new CoinhubWalletAdapter(),
    new MathWalletAdapter(),
    new SafePalWalletAdapter(),
    new SolongWalletAdapter(),
    new TokenPocketWalletAdapter(),
    new NufiWalletAdapter(),
    new AvanaWalletAdapter(),
    new BackpackWalletAdapter(),
    new BraveWalletAdapter(),
    new CoinbaseWalletAdapter(),
    new ExodusWalletAdapter(),
    new HuobiWalletAdapter(),
    new HyperPayWalletAdapter(),
    new KeystoneWalletAdapter(),
    new NekoWalletAdapter(),
    new NightlyWalletAdapter(),
    new SaifuWalletAdapter(),
    new SalmonWalletAdapter(),
    new SkyWalletAdapter(),
    new StrikeWalletAdapter(),
    new TokenaryWalletAdapter(),
    new TrustWalletAdapter()
]


function sendUnityEvent(eventName, ...args){
    window.unityInstance.SendMessage('JsEventDispatcher', 'OnHandleEvent', JSON.stringify([eventName, ...args]));
}

wallets.forEach(wallet => {
    wallet.on('connect', () => {
        console.log('Wallet connected: [' + wallet.name + '], address: [' + wallet.publicKey + ']');

        sendUnityEvent('OnWalletConnectedEvent', wallet.name, wallet.publicKey);
    });

    wallet.on('disconnect', () => {
        console.log('Wallet disconnected: [' + wallet.name + ']');

        sendUnityEvent('OnWalletDisconnectedEvent', wallet.name);
    });
});

let isDisconecting = false;

async function connect(adapter) {
    if (adapter.connecting || adapter.connected){
        return;
    }

	const readyState = adapter?.readyState;

    console.log('wallet [' + adapter.name + '] state is [' + readyState + ']');

	if (!(readyState === WalletReadyState.Installed || readyState === WalletReadyState.Loadable)) {

        if (typeof window !== 'undefined') {
            window.open(adapter.url, '_blank');
        }

        throw new WalletNotReadyError();
    }

    try {
        await adapter.connect();
        window.unityConfig.currentAdapter = adapter;
    } catch (error) {
        console.error('Wallet error: [' + adapter.name + '], error: [' + error + ']');

        sendUnityEvent('OnWalletErrorEvent', adapter.name);
    }
}

async function disconnect(adapter) {
    if (!adapter.connected) {
        return;
    }

    try {
        await adapter.disconnect();
    } catch (error) {
        throw error;
    } 
}

function connectWalletByName(walletName) {
    let walletIndex = wallets.findIndex(x => x.name === walletName);
    
    if (walletIndex < 0){
        sendUnityEvent('OnWalletErrorEvent', walletName);
        console.error('connectWalletByName(): Wallet with id [' + walletName + '] not found');
        return;
    }

    connect(wallets[walletIndex]);
}

function disconnectWalletByName(walletName) {
    let walletIndex = wallets.findIndex(x => x.name === walletName);
    
    if (walletIndex < 0){
        sendUnityEvent('OnWalletErrorEvent', walletName);
        console.error('connectWalletByName(): Wallet with id [' + walletName + '] not found');
        return;
    }

    disconnect(wallets[walletIndex]);
}

async function signMessage(adapter, messageStr){
    if (!adapter || !adapter.connected){
        console.error('Not connected');
        sendUnityEvent('OnMessageSignErrorEvent', adapter.name);
        return;
    }

    try {
        if (adapter && 'signMessage' in adapter){
            const message = new TextEncoder().encode(messageStr);
            const signature = await adapter.signMessage(message);
    
            const base64str = Buffer.from(signature).toString('base64');
    
            sendUnityEvent('OnWalletMessageSignedEvent', adapter.name, adapter.publicKey, messageStr, base64str);
        } else {
            console.error('Signing not supported with this wallet');
    
            sendUnityEvent('OnMessageSignErrorEvent', adapter.name);
        }  
    } catch(error){
        console.log("signMessage() exception:");
        console.log(error);

        sendUnityEvent('OnMessageSignErrorEvent', adapter.name);
    }
}

function signMessageWithWalletByName(walletName, messageStr) {
    let walletIndex = wallets.findIndex(x => x.name === walletName);
    
    if (walletIndex < 0){
        sendUnityEvent('OnMessageSignErrorEvent', walletName);
        console.error('signMessageWithWalletByName(): Wallet with id [' + walletName + '] not found');
        return;
    }

    signMessage(wallets[walletIndex], messageStr);
}

async function getSolanaMetadataAddress(tokenMint) {   
    const metaProgamPublicKey = new PublicKey(METADATA_PROGRAM);
    return (
        await PublicKey.findProgramAddress(
        [metaProgamPrefixBuffer, metaProgamPublicKeyBuffer, new PublicKey(tokenMint).toBuffer()],
        metaProgamPublicKey
        )
    )[0];
}


async function sendTransaction(adapter, base64Transaction){
    if (!adapter || !adapter.connected){
        console.error('Not connected');
        sendUnityEvent('OnSendTxErrorEvent', adapter.name);
        return;
    }

    try {
        if (adapter && 'sendTransaction' in adapter){
            let tx = Transaction.from(Buffer.from(base64Transaction, 'base64'))
            tx.recentBlockhash = (await connection.getLatestBlockhash()).blockhash;
            const signature = await adapter.sendTransaction(tx, connection);

            sendUnityEvent('OnTransactionSentEvent', adapter.name, signature);
        } else {
            console.error('sendTransaction() not found in this wallet adapter');
    
            sendUnityEvent('OnSendTxErrorEvent', adapter.name);
        }  
    } catch(error){
        console.log("sendTransaction() exception:");
        console.log(error);

        sendUnityEvent('OnSendTxErrorEvent', adapter.name);
    }
}

function sentTransactionWithWalletByName(walletName, base64Transaction) {
    let walletIndex = wallets.findIndex(x => x.name === walletName);
    
    if (walletIndex < 0){
        sendUnityEvent('OnSendTxErrorEvent', walletName);
        console.error('sentTransactionWithWalletByName(): Wallet with id [' + walletName + '] not found');
        return;
    }

    sendTransaction(wallets[walletIndex], base64Transaction);
}



async function getMintMetadata(mint) {
  try {
    const metaPubKey = await getSolanaMetadataAddress(mint);
    const ownedMetadata = await Metadata.load(connection, metaPubKey.toString());


    const nftName = ownedMetadata.data.data.name;
    const nftUri = ownedMetadata.data.data.uri;

    const res = {name: nftName, url: nftUri, mint: mint};
    const strRes = JSON.stringify(res);

    sendUnityEvent('OnMintMetadataLoadedEvent', nftName, nftUri, mint);
  } catch (err){
    console.error(err);
    console.log('Failed to fetch metadata');

    sendUnityEvent('OnMintMetadataFailedToLoadedEvent', mint);
  }
};


interface UnityConfig {
    currentAdapter: BaseMessageSignerWalletAdapter;
    getWalletDataJsonStr: any;
    connectWalletByName: any;
    disconnectWalletByName: any;
    signMessageWithWalletByName: any;
    requestMintMetadata: any;
    sendUnityEvent: any;
    sentTransactionWithWalletByName: any;
}

declare global {
    interface Window {
        unityConfig:any;
        unityInstance:any;
    }
}

const unityConfig: UnityConfig = {
    getWalletDataJsonStr: null,
    connectWalletByName: connectWalletByName,
    disconnectWalletByName: disconnectWalletByName,
    signMessageWithWalletByName: signMessageWithWalletByName,
    currentAdapter: null,
    requestMintMetadata: (mint) => getMintMetadata(mint),
    sendUnityEvent: sendUnityEvent,
    sentTransactionWithWalletByName: sentTransactionWithWalletByName,
};

unityConfig.getWalletDataJsonStr = () => {
    const walletData = []

    wallets.forEach(wallet => { 
        walletData.push({
            id: wallet.name,
            installed: wallet.readyState == WalletReadyState.Installed,
            canSign: 'signMessage' in wallet
        });
    });

    console.log(walletData);

    return JSON.stringify({walletSpecs: walletData});
};




window.unityConfig = unityConfig;


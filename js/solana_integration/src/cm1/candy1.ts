import { PublicKey } from '@solana/web3.js';
import {
    CandyMachineAccount,
    CANDY_MACHINE_PROGRAM,
    getCandyMachineState,
    mintOneToken,
  } from './candy-machine';

  import {
    Commitment,
    Connection,
    SignatureStatus,
    TransactionSignature,
    TransactionResponse
  } from '@solana/web3.js';

import {awaitTransactionSignatureConfirmation} from './connection'

import { toDate, formatNumber } from './utils';
import { connection } from '../index'


const CANDY_ID = '8DNPZEpsJ9kdk48rYPW96uryi7yKSxujr4wEAJcQiyva';

let sendUnityEvent = window.unityConfig.sendUnityEvent;

async function getCandyMachineInfo(candy_id) {
    const candyMachine = await getCandyMachineState(window.unityConfig.currentAdapter, new PublicKey(candy_id), connection);

    const remaining = candyMachine?.state.itemsRemaining;
    const total = candyMachine?.state.itemsAvailable;
    const minted = candyMachine?.state.itemsRedeemed;
    const price = candyMachine?.state.price.toNumber()/1000000000;
    const isSoldOut = candyMachine?.state.isSoldOut;
    const isPresale = candyMachine?.state.isPresale ? true : false;

    const goLiveDate = toDate(candyMachine?.state.goLiveDate);


    sendUnityEvent('OnCandyMachineInfoEvent', {
      total: total,
      minted: minted,
      remaining: remaining,
      price: price,
      isSoldOut: isSoldOut,
      isPresale: isPresale,
      goLiveDate: goLiveDate
    });
}


async function mint(candy_id) {
  const txTimeout = 65000;
  const wallet = window.unityConfig.currentAdapter;
  let mintTxId;

  try {
    const candyMachine = await getCandyMachineState(window.unityConfig.currentAdapter, new PublicKey(candy_id), connection);

    if (wallet.connected && candyMachine?.program && wallet.publicKey) {
      mintTxId = (await mintOneToken(candyMachine, wallet.publicKey))[0];

      let status: any = { err: true };

      // sendUnityEvent('OnMintSuccessEvent', mintTxId);
      // return;

      if (!mintTxId){
        sendUnityEvent('OnMintFailedEvent', 'error', mintTxId);
        return;
      }

      sendUnityEvent('OnMintSuccessEvent', mintTxId);

      // let finalized = await awaitTransactionFinalization(mintTxId, txTimeout, connection);

      // if (finalized){
      //   sendUnityEvent('OnMintSuccessEvent', mintTxId);
      // } else {
      //   sendUnityEvent('OnMintFailedEvent', 'error', mintTxId);
      // }

      // if (mintTxId) {
      //   status = await awaitTransactionSignatureConfirmation(mintTxId,txTimeout,connection, 'finalized', true);
      // }


      // if (status && !status.err) {
      //   sendUnityEvent('OnMintSuccessEvent', mintTxId);
      // } else {
      //   sendUnityEvent('OnMintFailedEvent', 'error', mintTxId);
      // }
    } else {
        sendUnityEvent('OnMintFailedEvent', 'walletOrCandyError', mintTxId);
    }
  } catch (error: any) {
    let message = error.msg || 'Minting failed! Please try again!';
    if (!error.msg) {
      if (!error.message) {
        sendUnityEvent('OnMintFailedEvent', 'timeout', mintTxId);
      } else if (error.message.indexOf('0x137')) {
        sendUnityEvent('OnMintFailedEvent', 'soldout', mintTxId);
      } else if (error.message.indexOf('0x135')) {
        sendUnityEvent('OnMintFailedEvent', 'nofunds', mintTxId);
      } else {
        sendUnityEvent('OnMintFailedEvent', 'error', mintTxId);
      }
    } else {
      if (error.code === 311) {
        sendUnityEvent('OnMintFailedEvent', 'soldout', mintTxId);
      } else if (error.code === 312) {
        sendUnityEvent('OnMintFailedEvent', 'notstarted', mintTxId);
      } else {
        sendUnityEvent('OnMintFailedEvent', 'error', mintTxId);
      }
    }
  }
};


window.unityConfig.getCandyData = (cmId) => getCandyMachineInfo(cmId);
window.unityConfig.mintNft = (cmId) => mint(cmId);


async function awaitTransactionFinalization(
  txid: TransactionSignature,
  timeout: number,
  connection: Connection
): Promise<boolean | null | void> {
  let done = false;
  let status: TransactionResponse | null | void = null;

  status = await new Promise(async (resolve, reject) => {

    setTimeout(() => {
      if (done) {
        return;
      }
      done = true;
      console.log('Rejecting for timeout...');
      reject(false);
    }, timeout);

   
    while (!done) {
      (async () => {
        try {
          status = await connection.getTransaction(txid, {commitment: 'finalized'});
          if (!done) {
            if (status) {
              resolve(status);
            }
          }
        } catch (e) {
          if (!done) {
            console.log('REST connection error: txid', txid, e);
          }
        }
      })();
      await sleep(2000);
    }
  });

  //@ts-ignore
  // if (connection._signatureSubscriptions[subId])
  //   connection.removeSignatureListener(subId);
  done = true;
  console.log('Returning status', status);
  return status != null;
}
export function sleep(ms: number): Promise<void> {
  return new Promise(resolve => setTimeout(resolve, ms));
}
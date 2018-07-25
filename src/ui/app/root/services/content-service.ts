import { default as Axios } from "axios";
import { Store } from "vuex";
import { GlobalConfig } from "../../common";
import { IPayload } from "../../model";
import { RootStoreTypes } from "../store";
import { StoreService } from "../../store";

const BASE_URL = GlobalConfig.uri.content;

export class ContentService extends StoreService {
  constructor(store: Store<{}>) {
    super(store);
  }

  rikerIpsum() {
    let payload: IPayload<string> = null;
    let uri =
      BASE_URL +
      "rikeripsum/?clientTime=" +
      encodeURIComponent(new Date().toISOString());

    return this.exec<string>(Axios.get(uri))
      .then(value => {
        payload = value;
        return this.store.dispatch(RootStoreTypes.apiCallContent, value.data);
      })
      .then(() =>
        this.store.dispatch(
          RootStoreTypes.common.updateStatusBar,
          payload.message
        )
      );
  }

  secureUserContent(): Promise<string> {
    var uri = BASE_URL + "secureUserContent";

    return this.exec<string>(Axios.get(uri, <any>{ ignore: [403] })).then(
      value => this.processPayload(value)
    );
  }
}
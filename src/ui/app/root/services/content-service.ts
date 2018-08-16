import { default as Axios } from "axios";
import { Store } from "vuex";
import { GlobalConfig } from "../../common";
import { IPayload } from "../../model";
import { RootStoreTypes } from "../store";
import { StoreService } from "../../store";

const RIKER_IPSUM = GlobalConfig.uri.services + "content/rikeripsum";

export class ContentService extends StoreService {
  constructor(store: Store<{}>) {
    super(store);
  }

  rikerIpsum() {
    let payload: IPayload<string> = null;
    let dt = new Date();
    let uri =
      RIKER_IPSUM +
      "/?default=" +
      encodeURIComponent(dt.toString()) +
      "&locale=" +
      encodeURIComponent(dt.toLocaleString()) +
      "&iso=" +
      encodeURIComponent(dt.toISOString()) +
      "&utc=" +
      encodeURIComponent(dt.toUTCString());

    console.log(uri);

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
}

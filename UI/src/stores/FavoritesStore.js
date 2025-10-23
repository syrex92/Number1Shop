import { makeAutoObservable } from 'mobx';

export class FavoritesStore {
  ids = new Set();

  constructor() {
    makeAutoObservable(this, {}, { autoBind: true });
  }

  isFavorite(id) {
    return this.ids.has(id);
  }

  toggle(id) {
    if (this.ids.has(id)) this.ids.delete(id);
    else this.ids.add(id);
  }
}

export default FavoritesStore;



import { makeAutoObservable } from 'mobx';

export interface FavoritesStore {
  ids: Set<string>;
  isFavorite: (id: string) => boolean;
  toggle: (id: string) => void;
}

export const createFavoritesStore = (): FavoritesStore => {
  const store = {
    ids: new Set<string>(),

    isFavorite(id: string): boolean {
      return this.ids.has(id);
    },

    toggle(id: string): void {
      if (this.ids.has(id)) {
        this.ids.delete(id);
      } else {
        this.ids.add(id);
      }
    }
  };

  return makeAutoObservable(store);
};

export default createFavoritesStore;
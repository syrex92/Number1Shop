import { makeAutoObservable } from 'mobx';

export class AuthStore {
  user = null; // { id, name, role }
  isLoading = false;
  error = null;

  constructor() {
    makeAutoObservable(this, {}, { autoBind: true });
  }

  get isAuthenticated() {
    return !!this.user;
  }

  get role() {
    return this.user?.role || 'guest';
  }

  async login(email, password) {
    this.isLoading = true;
    this.error = null;
    try {
      // Mock login
      await new Promise(r => setTimeout(r, 300));
      this.user = { id: '1', name: 'Клиент', role: 'customer', email };
    } catch (e) {
      this.error = 'Ошибка авторизации';
    } finally {
      this.isLoading = false;
    }
  }

  logout() {
    this.user = null;
  }
}

export default AuthStore;



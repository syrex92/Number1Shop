import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { makeAutoObservable, reaction, runInAction } from "mobx";
import { notifications as mantineNotifications } from "@mantine/notifications";
import shopConfig from "../config/shopConfig";
import type { AuthStore } from "./AuthStore";

export interface UiNotificationEnvelope {
  id: string;
  type: string;
  title: string;
  message: string;
  createdAt: string;
  userId?: string | null;
  data?: unknown;
}

export interface UiNotification extends UiNotificationEnvelope {
  read: boolean;
}

const STORAGE_BASE_KEY = "shop.notifications.v1";

function getStorageKey(userId?: string | null): string {
  return userId ? `${STORAGE_BASE_KEY}:${userId}` : STORAGE_BASE_KEY;
}

function safeParse<T>(value: string | null): T | null {
  if (!value) return null;
  try {
    return JSON.parse(value) as T;
  } catch {
    return null;
  }
}

export const createNotificationStore = (
  auth: AuthStore,
  onNotification?: (envelope: UiNotificationEnvelope) => void
) => {
  const store = {
    items: [] as UiNotification[],
    isConnected: false,
    lastError: null as string | null,

    _connection: null as HubConnection | null,
    _started: false,

    get unreadCount() {
      return this.items.filter((x) => !x.read).length;
    },

    loadFromStorage() {
      const key = getStorageKey(auth.user?.id);
      const saved = safeParse<UiNotification[]>(localStorage.getItem(key));
      if (saved && Array.isArray(saved)) {
        this.items = saved.slice(0, 50);
      }
    },

    saveToStorage() {
      try {
        const key = getStorageKey(auth.user?.id);
        localStorage.setItem(key, JSON.stringify(this.items.slice(0, 50)));
      } catch {
        // ignore storage errors
      }
    },

    add(envelope: UiNotificationEnvelope) {
      const item: UiNotification = {
        ...envelope,
        read: false,
      };
      this.items = [item, ...this.items].slice(0, 50);

      mantineNotifications.show({
        title: envelope.title,
        message: envelope.message,
        autoClose: 6000,
      });

      if (onNotification) {
        try {
          onNotification(envelope);
        } catch {
          // ignore side-effect errors
        }
      }
    },

    markRead(id: string) {
      this.items = this.items.map((x) => (x.id === id ? { ...x, read: true } : x));
    },

    markAllRead() {
      this.items = this.items.map((x) => ({ ...x, read: true }));
    },

    clear() {
      this.items = [];
    },

    async connect() {
      if (this._started) return;
      if (!auth.accessToken || !auth.isAuthenticated) return;

      const hubUrl = `${shopConfig.siteUrl}hubs/notifications`;

      const connection = new HubConnectionBuilder()
        .withUrl(hubUrl, {
          accessTokenFactory: () => auth.accessToken ?? "",
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      connection.on("notification", (payload: UiNotificationEnvelope) => {
        runInAction(() => this.add(payload));
      });

      connection.onreconnected(() => {
        runInAction(() => {
          this.isConnected = true;
          this.lastError = null;
        });
      });

      connection.onclose((err) => {
        runInAction(() => {
          this.isConnected = false;
          if (err) this.lastError = err.message;
        });
      });

      this._connection = connection;
      this._started = true;

      try {
        await connection.start();
        runInAction(() => {
          this.isConnected = connection.state === HubConnectionState.Connected;
          this.lastError = null;
        });
      } catch (e) {
        runInAction(() => {
          this.isConnected = false;
          this.lastError = e instanceof Error ? e.message : "SignalR connection failed";
        });
      }
    },

    async disconnect() {
      this._started = false;
      this.isConnected = false;
      this.lastError = null;

      if (!this._connection) return;
      try {
        await this._connection.stop();
      } finally {
        this._connection = null;
      }
    },
  };

  makeAutoObservable(store);

  store.loadFromStorage();

  reaction(
    () => store.items.map((x) => ({ id: x.id, read: x.read, createdAt: x.createdAt })),
    () => store.saveToStorage()
  );

  reaction(
    () => auth.accessToken,
    async () => {
      if (auth.accessToken && auth.isAuthenticated) {
        // при успешной аутентификации подгружаем сохранённые уведомления этого пользователя
        runInAction(() => {
          store.clear();
          store.loadFromStorage();
        });
        await store.connect();
      } else {
        await store.disconnect();
        runInAction(() => {
          store.clear(); // очищаем только in-memory, localStorage не трогаем
        });
      }
    },
    { fireImmediately: true }
  );

  // Переключение пользователя: отдельное хранилище уведомлений на каждого.
  reaction(
    () => auth.user?.id,
    () => {
      runInAction(() => {
        store.clear();
      });
      store.loadFromStorage();
    }
  );

  return store;
};

export type NotificationStore = ReturnType<typeof createNotificationStore>;


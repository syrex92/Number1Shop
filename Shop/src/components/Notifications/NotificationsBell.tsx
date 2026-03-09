import { observer } from "mobx-react-lite";
import { ActionIcon, Badge, Button, Group, Indicator, Popover, ScrollArea, Stack, Text, Title } from "@mantine/core";
import { IconBell, IconCheck, IconTrash } from "@tabler/icons-react";
import { useStores } from "../../context/RootStoreContext";

function formatTime(iso: string) {
  try {
    return new Date(iso).toLocaleString();
  } catch {
    return iso;
  }
}

const NotificationsBell = observer(() => {
  const { notifications } = useStores();
  const unread = notifications.unreadCount;
  const unreadLabel = unread > 99 ? "99+" : String(unread);

  return (
    <Popover width={360} position="bottom-end" withArrow shadow="md">
      <Popover.Target>
        <ActionIcon variant="subtle" color="gray" aria-label="Notifications">
          <Indicator
            disabled={unread <= 0}
            label={unreadLabel}
            size={16}
            offset={4}
            position="top-end"
            color="red"
            withBorder
          >
            <IconBell size={20} />
          </Indicator>
        </ActionIcon>
      </Popover.Target>

      <Popover.Dropdown>
        <Stack gap="xs">
          <Group justify="space-between" align="center">
            <Title order={5}>Уведомления</Title>
            <Badge size="sm" variant="light" color={notifications.isConnected ? "green" : "gray"}>
              {notifications.isConnected ? "online" : "offline"}
            </Badge>
          </Group>

          <Group gap="xs">
            <Button
              size="xs"
              variant="light"
              leftSection={<IconCheck size={14} />}
              onClick={() => notifications.markAllRead()}
              disabled={notifications.items.length === 0}
            >
              Прочитано
            </Button>
            <Button
              size="xs"
              variant="light"
              color="red"
              leftSection={<IconTrash size={14} />}
              onClick={() => notifications.clear()}
              disabled={notifications.items.length === 0}
            >
              Очистить
            </Button>
          </Group>

          {notifications.items.length === 0 ? (
            <Text c="dimmed" size="sm">
              Пока пусто
            </Text>
          ) : (
            <ScrollArea h={320} type="auto">
              <Stack gap="sm">
                {notifications.items.map((n) => (
                  <div
                    key={n.id}
                    style={{
                      padding: 10,
                      borderRadius: 10,
                      border: "1px solid rgba(255,255,255,0.08)",
                      background: n.read ? "transparent" : "rgba(34,139,230,0.10)",
                      cursor: "pointer",
                    }}
                    onClick={() => notifications.markRead(n.id)}
                  >
                    <Group justify="space-between" align="flex-start" gap="xs">
                      <Text fw={600} size="sm">
                        {n.title}
                      </Text>
                      <Text c="dimmed" size="xs">
                        {formatTime(n.createdAt)}
                      </Text>
                    </Group>
                    <Text size="sm" c="dimmed">
                      {n.message}
                    </Text>
                  </div>
                ))}
              </Stack>
            </ScrollArea>
          )}
        </Stack>
      </Popover.Dropdown>
    </Popover>
  );
});

export default NotificationsBell;


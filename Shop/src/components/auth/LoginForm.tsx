import { useState } from 'react';
import { observer } from 'mobx-react-lite';
import { Paper, LoadingOverlay, Alert } from '@mantine/core';
import { IconAlertCircle } from '@tabler/icons-react';
import { useStores } from "../../context/RootStoreContext.tsx";
import LoginFormFields from './LoginFormFields.tsx';
import type { LoginFormData } from './LoginFormFields.tsx';

const LoginForm = observer(() => {
  const { auth } = useStores();
  const [formData, setFormData] = useState<LoginFormData>({
    email: '',
    password: '',
    rememberMe: false,
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (validateForm(formData)) {
      await auth.login(formData.email, formData.password);
    }
  };

  return (
    <Paper withBorder shadow="md" p={30} mt={30} radius="md" pos="relative">
      <LoadingOverlay visible={auth.isLoading} />
      
      <form onSubmit={handleSubmit}>
        <LoginFormFields formData={formData} onFormDataChange={setFormData}/>

        {auth.error && (
          <Alert icon={<IconAlertCircle size={16} />} title="Ошибка" color="red" mt="md">
            {auth.error}
          </Alert>
        )}
      </form>
    </Paper>
  );
});

// Валидация формы
const validateForm = (formData: LoginFormData): boolean => {
  if (!formData.email) return false;
  if (!/^\S+@\S+$/.test(formData.email)) return false;
  if (!formData.password) return false;
  if (formData.password.length < 6) return false;
  return true;
};

export default LoginForm;
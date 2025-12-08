import { TextInput, PasswordInput, Button, Stack } from '@mantine/core';

export interface RegistrationFormData {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
}

interface RegistrationFormFieldsProps {
  formData: RegistrationFormData;
  onFormDataChange: (data: RegistrationFormData) => void;
}

const RegistrationFormFields = ({ formData, onFormDataChange }: RegistrationFormFieldsProps) => {
  
  const handleInputChange = (field: keyof RegistrationFormData, value: string) => {
    onFormDataChange({
      ...formData,
      [field]: value
    });
  };

  const isFormValid = formData.name &&
    formData.name.length >= 2 &&
    formData.email &&
    /^\S+@\S+$/.test(formData.email) &&
    formData.password &&
    formData.password.length >= 6 &&
    formData.confirmPassword &&
    formData.password === formData.confirmPassword;

  return (
    <Stack>
      <TextInput
        label="Имя"
        placeholder="Ваше имя"
        required
        value={formData.name}
        onChange={(e) => handleInputChange('name', e.target.value)}
        error={formData.name && formData.name.length < 2 ? 'Имя должно содержать минимум 2 символа' : null}
        description="Минимум 2 символа"
      />

      <TextInput
        label="Email"
        placeholder="your@email.com"
        required
        value={formData.email}
        onChange={(e) => handleInputChange('email', e.target.value)}
        error={formData.email && !/^\S+@\S+$/.test(formData.email) ? 'Некорректный email' : null}
      />

      <PasswordInput
        label="Пароль"
        placeholder="Ваш пароль"
        required
        value={formData.password}
        onChange={(e) => handleInputChange('password', e.target.value)}
        error={formData.password && formData.password.length < 6 ? 'Пароль должен содержать минимум 6 символов' : null}
        description="Минимум 6 символов"
      />

      <PasswordInput
        label="Подтверждение пароля"
        placeholder="Повторите пароль"
        required
        value={formData.confirmPassword}
        onChange={(e) => handleInputChange('confirmPassword', e.target.value)}
        error={formData.confirmPassword && formData.password !== formData.confirmPassword ? 'Пароли не совпадают' : null}
      />

      <Button
        type="submit"
        fullWidth
        mt="xl"
        disabled={!isFormValid}
      >
        Зарегистрироваться
      </Button>
    </Stack>
  );
};

export default RegistrationFormFields;
import {
    TextInput,
    PasswordInput,
    Group,
    Button,
    Checkbox,
    Anchor,
    Stack,
} from '@mantine/core';

interface LoginFormFieldsProps {
    formData: LoginFormData;
    onFormDataChange: (data: LoginFormData) => void;
}
export interface LoginFormData {
    email: string;
    password: string;
    rememberMe: boolean;
}

const LoginFormFields = ({ formData, onFormDataChange }: LoginFormFieldsProps) => {
    const handleInputChange = (field: keyof LoginFormData, value: string | boolean) => {
        onFormDataChange({
            ...formData,
            [field]: value
        });
    };

    const isFormValid = formData.email &&
        /^\S+@\S+$/.test(formData.email) &&
        formData.password &&
        formData.password.length >= 6;

    return (
        <Stack>
            <TextInput label="Email" placeholder="your@email.com"
                required
                value={formData.email}
                onChange={(e) => handleInputChange('email', e.target.value)}
                error={formData.email && !/^\S+@\S+$/.test(formData.email) ? 'Некорректный email' : null} />
            <PasswordInput label="Пароль" placeholder="Ваш пароль"
                required
                value={formData.password}
                onChange={(e) => handleInputChange('password', e.target.value)}
                error={formData.password && formData.password.length < 6 ? 'Пароль должен содержать минимум 6 символов' : null} />
            <Group justify="space-between">
                <Checkbox label="Запомнить меня" checked={formData.rememberMe}
                    onChange={(e) => handleInputChange('rememberMe', e.currentTarget.checked)} />
                <Anchor component="button" type="button" size="sm" onClick={() => {/* Добавьте логику восстановления пароля */ }}>
                    Забыли пароль?
                </Anchor>
            </Group>
            <Button type="submit" fullWidth mt="xl" disabled={!isFormValid} >
                Войти
            </Button>
        </Stack>
    );
};

export default LoginFormFields;
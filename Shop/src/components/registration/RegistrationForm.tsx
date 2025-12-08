import { useState } from 'react';
import { observer } from 'mobx-react-lite';
import { Paper, LoadingOverlay, Alert } from '@mantine/core';
import { IconAlertCircle } from '@tabler/icons-react';
import { useStores } from "../../context/RootStoreContext.tsx";
import RegistrationFormFields from './RegistrationFormFields.tsx';
import type { RegistrationFormData } from './RegistrationFormFields.tsx';

const RegistrationForm = observer(() => {
    const { auth } = useStores();
    const [formData, setFormData] = useState<RegistrationFormData>({
        name: '',
        email: '',
        password: '',
        confirmPassword: '',
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (validateForm(formData)) {
            await auth.registration(formData);
        }
    };

    return (
        <Paper withBorder shadow="md" p={30} mt={30} radius="md" pos="relative">
            <LoadingOverlay visible={auth.isLoading} />

            <form onSubmit={handleSubmit}>
                <RegistrationFormFields formData={formData} onFormDataChange={setFormData} />

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
const validateForm = (formData: RegistrationFormData): boolean => {
    if (!formData.name || formData.name.length < 2) return false;
    if (!formData.email || !/^\S+@\S+$/.test(formData.email)) return false;
    if (!formData.password || formData.password.length < 6) return false;
    if (!formData.confirmPassword || formData.password !== formData.confirmPassword) return false;
    return true;
};

export default RegistrationForm;
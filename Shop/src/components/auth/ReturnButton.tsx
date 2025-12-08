import { Button } from '@mantine/core';
import { IconArrowLeft } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';

const ReturnButton = () => {
    const navigate = useNavigate();

    const handleReturn = () => {
        navigate("/");
    }

    return (
        <Button color="gray" onClick={handleReturn} variant={"transparent"} fullWidth mt="xl" leftSection={<IconArrowLeft size={16} />}>
            "Вернуться в магазин"
        </Button>
    );
}

export default ReturnButton;
import { Button, CloseButton, Flex, Input, NumberInput } from "@mantine/core";
import type { NewProduct, Product } from "../../../stores/ProductsStore";
import { useState } from "react";

interface INewProductProps {
  product: Product | undefined;
  onConfirm(newProduct: NewProduct | Product, isCreate: boolean): void;
  onCancel(): void;
}

const ProductComponent = ({
  product,
  onConfirm,
  onCancel,
}: INewProductProps) => {
  const ErrorText = "Обязательно для заполнения";

  const [title, setTitle] = useState<string>(product?.title ?? "");
  const [titleError, setTitleError] = useState<string | undefined>(undefined);
  const [description, setDescription] = useState<string>(
    product?.description ?? ""
  );
  const [article, setArticle] = useState<string | number>(product?.article ?? 0);
  const [articleError, setArticleError] = useState<string | undefined>(undefined);
  const [category, setCategory] = useState<string>(product?.category ?? "");
  const [categoryError, setCategoryError] = useState<string | undefined>(
    undefined
  );
  const [price, setPrice] = useState<string | number>(product?.price ?? 0);

  const handleConfirm = () => {
    if (!title || title == "") {
      setTitleError(ErrorText);
    }
    if (article === 0) {
      setArticleError("Не может быть 0");
    }
    if (!category || category == "") {
      setCategoryError(ErrorText);
    }

    if (!title || !article || !category) {
      return;
    }

    if (!product) {
      const newProduct: NewProduct = {
        article: Number(article),
        price: Number(price),
        title: title,
        description: description,
        category: category,
        image: undefined,
      };
      onConfirm(newProduct, true);
    } else {
      const updatedProduct: Product = {
        id: product.id,
        article: Number(article),
        price: Number(price),
        title: title,
        description: description,
        category: category,
        image: undefined,
      };
      onConfirm(updatedProduct, false);
    }
    onCancel();
  };

  return (
    <>
      <Flex
        mih={50}
        gap="md"
        justify="center"
        align="flex-start"
        direction="column"
        wrap="wrap"
      >
        <Input.Wrapper
          label="Название товара"
          description="Заполните название товара"
          error={titleError}
        >
          <Input
            placeholder="Название товара"
            value={title}
            onChange={(event) => {
              setTitleError(undefined);
              setTitle(event.currentTarget.value);
            }}
            rightSectionPointerEvents="all"
            mt="md"
            rightSection={
              <CloseButton
                aria-label="Clear input"
                onClick={() => setTitle("")}
                style={{ display: title ? undefined : "none" }}
              />
            }
          />
        </Input.Wrapper>

        <Input.Wrapper
          label="Описание товара"
          description="Заполните описание товара"
        >
          <Input
            placeholder="Описание товара"
            value={description}
            onChange={(event) => setDescription(event.currentTarget.value)}
            rightSectionPointerEvents="all"
            mt="md"
            rightSection={
              <CloseButton
                aria-label="Clear input"
                onClick={() => setDescription("")}
                style={{ display: description ? undefined : "none" }}
              />
            }
          />
        </Input.Wrapper>

        <NumberInput
          label="Артикул товара"
          description="Заполните артикул товара"
          placeholder="Артикул"
          error={articleError}
          value={article}
          onChange={setArticle}
          rightSectionPointerEvents="all"
          mt="md"
          rightSection={
            <CloseButton
              aria-label="Clear input"
              onClick={() => { setArticleError(undefined); setArticle(0);}}
              style={{ display: article ? undefined : "none" }}
            />
          }
        />

        <Input.Wrapper
          label="Категория товара"
          description="Заполните категорию товара"
          error={categoryError}
        >
          <Input
            placeholder="Категория"
            value={category}
            onChange={(event) => {
              setCategoryError(undefined);
              setCategory(event.currentTarget.value);
            }}
            rightSectionPointerEvents="all"
            mt="md"
            rightSection={
              <CloseButton
                aria-label="Clear input"
                onClick={() => setCategory("")}
                style={{ display: category ? undefined : "none" }}
              />
            }
          />
        </Input.Wrapper>

        <NumberInput
          label="Цена товара"
          description="Заполните цену товара"
          placeholder="Цена"
          value={price}
          onChange={setPrice}
          min={0}
          rightSectionPointerEvents="all"
          mt="md"
          rightSection={
            <CloseButton
              aria-label="Clear input"
              onClick={() => setPrice(0)}
              style={{ display: price ? undefined : "none" }}
            />
          }
        />
        <Flex
          mih={50}
          gap="md"
          justify="center"
          align="center"
          direction="row"
          wrap="wrap"
        >
          <Button onClick={handleConfirm}>Подтвердить</Button>
          <Button onClick={onCancel}>Отменить</Button>
        </Flex>
      </Flex>
    </>
  );
};

export default ProductComponent;

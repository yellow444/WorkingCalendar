import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import common_ru from "./translations/ru/common.json";
import common_en from "./translations/en/common.json";
const resources = {
    en: {
        translation: common_en
    },
    ru: {
        translation: common_ru
    }
};

i18n
    .use(initReactI18next)
    .init({
        resources,
        lng: 'ru',
        fallbackLng: 'en'
    });
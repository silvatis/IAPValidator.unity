Пример использования можно найти в папке CrossLink/Example, сцена CrossLinkExample. Тестовый скрипт CrossLinkExample.cs привязан к объекту Canvas.
1) Добавить на сцену показа рекламы префаб CrossLinkPrefab из папки CrossLink/Prefabs.
3) Иерархия сцены -> объект CrossLinkPrefab -> компонент Cross Link -> параметр Show Confirmation выставляем для своей игры (true - показывать окно подтверждения перехода на сайт игры). 
4) Папка CrossLink/Resources -> заменяем картину border (рамка) на свою.
5) Иерархия сцены -> Объект CrossLinkPrefab -> child Border -> Выставляем значения Rect Transform рамки под сою игру.
5) На старте игры (скрипт инициализации, скрипт показа логотипов или др.) вызываем CrossLink.Init(int gameID, int providerID), где gameID - id вашей игры, providerID - id провайдера (629-Google, 931-Apple).
6) CrossLink.Show(), CrossLink.Hide() - показывать/скрывать баннер.

ВАЖНО! Согласно конфигурации безопасности сети (https://developer.android.com/training/articles/security-config), начиная с Android 9 (target SDK 28), протокол HTTP по умолчанию отключен.
Для показа анимированного кросс-промо необходимо сделать следующее:

Решение №1.

1. Создать файл "Plugins/Android/res/xml/network_security_config.xml" (Уже добавлен в пакет):
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
<domain-config cleartextTrafficPermitted="true">
<domain includeSubdomains="true">adv.herocraft.com</domain>
<domain includeSubdomains="true">res.herocraft.com</domain>
</domain-config>
</network-security-config>

2. Файл AndroidManifest.xml -> в таг application добавить строку android:networkSecurityConfig="@xml/network_security_config".

Решение №2.

Файл AndroidManifest.xml -> в таг application добавить строку android:usesCleartextTraffic="true".
# Таск-трекер

**CRUD** приложение для добавления и отслеживания задач.

## Основные функции:
1. **Добавление и завершение задач**:
   - Реализована возможность добавления новых задач.
   - Возможность отмечать задачи как завершенные.

2. **Формирование отчетов**:
   - Автоматическое формирование `.csv` файла с результатами задач, добавленных за день.

3. **Поиск задач**:
   - Поиск задач по названию и сложности.

4. **Пагинация**:
   - Реализована пагинация для удобного отображения данных.

## Архитектура:
При разработке использовалась **луковичная архитектура** (Onion Architecture), что обеспечивает:
- Удобство разработки.
- Легкость масштабирования приложения в будущем.

![](https://habrastorage.org/r/w1560/getpro/habr/upload_files/900/2b9/727/9002b9727fba0bcf68db8b9e797ead34.jpg)
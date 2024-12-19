HexBattler

Прототип сцены пошагового сражения на гексагональной карте.
Цель: создание сцены для пошаговых сражений с проработанными базовыми механиками, на основе которых можно создать любые более сложные.

Технический статус:
  - Проект реализован с использованием примитивов, нацелен на тестирование и демонстрацию механик.
  - Тестируется в режиме hot seat.
  - Генерация карты и базовые механики полностью функциональны.
  - В разработке: AI и способности классов. 

1. Очерёдность генерации карты
  1. Центральный гекс с флагом.
  2. Концентрические кругов из гексов. Гексы делятся на проходимые (обычный, труднопроходимый, высотный, глубинный) и непроходимые (частичные и полные укрытия). Количество кругов можно настраивать.
  3. Дополнительный круг, состоящий из непроходимых тайлов.
  4. На противоположных сторонах последнего проходимого круга создаются два отряда юнитов.
  5. Определяется значение инициативы юнитов, действует до конца сражения.

2. Система способностей и классов
Реализованы 4 базовые способности:
  - Движение
  - Ближняя атака: устанавливается шанс попадания. Основа для способностей, направленных на врагов, стоящих рядом.
  - Дальняя атака: дополнительно устанавливается дальность и тип области поражения. Не может быть применена на стояющую рядом цель. Опционально, с шансом 5% атака вместо основной цели может попасть по побочной (стоящей на соседнем тайле с основной целью). Наличие нескольких побочных целей повышает этот шанс. Основа для способностей, направленных на врагов, исключая стоящих рядом.
  - Защита (увеличение шанса уклониться атаки): используется как основа для способностей, направленных на себя и на союзников.

Параметры способностей:
  - Тип цели: Враг, союзник (включая или исключая себя), тайл.
  - Область поражения: гекс (круг с нулевым радиусом), круг, луч, конус, дуга (дальняя грань конуса). Для гекса допускается последовательный выбор нескольких целей. 
  - Эффекты: изменение параметров юнитов, перемещение.

Параметры юнита (класса):
  - Здоровье
  - Очки действия
  - Дальность перемещения
  - Инициатива (выбирается между минимальным и максимальным значением)
  - Шанс попасть по цели
  - Шанс уклониться от атаки

Классовые способности (в разработке): у каждого класса предполагается по две классовых способностей, которые будут созданы на логике базовых

3. Особенности тайлов:
- Обычный тайл (шанс создания 40%) - нет эффектов
- Труднопроходимый тайл (10%) - расходует два очка перемещения при проходе через него
- Высотный тайл (15%) - повышает шанс стоящего на нём юнита попасть по цели, но понижает его шанс уклонения от вражеских атак.
- Глубинный тайл (15%) - наоборот.
- Частичное укрытие (10%) - повышает шанс юнита уклониться от дальних атак, если укрытие находится между атакующим и целью.
- Полное укрытие (10%) - работает аналогично, но эффективнее. 

Центральный тайл с флагом всегда обычный.

4. Условия победы:
- Уничтожение всех юнитов противника
- Захват и удержание флага в течение трёх раундов. Для удержания юнит одной из команд должен находиться либо на тайле флага, либо на соседнем с ним тайле. Если захват ведут обе команды, очко захвата по итогам раунда не присваивается никому.

5. AI (в разработке)
Общие правила: при прочих равных юниты стремятся атаковать с высотного тайла, а ход завершать за укрытием или на глубинном тайле. В случае ранения - отступают или пытаются подойти ближе к жрецу.

Классы условно делятся на 3 группы:
  1. Воин
    - Часто: стремится подобраться к вражескими стрелкам, чтобы вовлечь их в ближний бой.
    - Реже: стремится защитить союзных стрелков от вражеских воинов.
    - Ещё реже: атакует вражеского жреца, или защищает его, если он атакован.
    - Если атакован: чаще сражается до конца, реже пытается подобраться к жрецу.

  2. Стрелок
    - Часто: стремится занять высоту и атакует наиболее уязвимого/раненого юнита
    - Реже: Атакует юнита, который напал на союзника, принуждая того отступить.
    - Ещё реже: Занимает оборонительную позицию в глубинном тайле, атакуя лучшую из доступных целей.
    - Если атакован: чаще пытается сменить позицию, реже переходит в ближний бой.

  2.1. Волшебник
    - Действует по сценарию стрелка, но более оборонительно, предпочитая низинные тайлы. Выжидает удобного момента для атаки по области, иногда пренебрегая тем, что его атака заденет союзника. Старается избегать угрозы вовлечения в ближний бой.

  3. Поддержка (жрец)
    - Часто: держит позицию между линиями, готовясь организовать численное преимущество или помочь раненому там, где это нужно. Также это полезно для лечения/баффа, накладываемого на несколько целей сразу.
    - Реже: вынужденно ломает строй, переходя в переднюю или заднюю линию.
    - Ещё реже: после израсходования всех лечебных заклинаний, действует более самоотверженно, отвлекая врагов от "более полезных" союзников.
    - Если атакован: чаще пытается отступить к союзникам.

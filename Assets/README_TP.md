# TP SnackLine – Ligne de production

## Le projet

Vous allez construire une **usine alimentaire automatisée** : des sources produisent des ingrédients, des convoyeurs les transportent, des machines les transforment et les assemblent, et les produits finis sont vendus en bout de chaîne.

```
Source (steak cru) → convoyeurs → Four → convoyeurs → Assembleur → convoyeurs → Vente
Source (pain)      → convoyeurs ─────────────────────↗
```

## Avant de commencer

- Version de Unity : **6000.3.23f1**.
- Package **Recorder** : déjà installé (il sert à filmer votre usine, voir les bonus).
- Ouvrez la scène `Assets/Scenes/SnackLine_Exemple.unity` et lancez Play : des steaks partent de la source, suivent les convoyeurs et sont vendus. La Console affiche chaque vente.
- Votre scène de travail est `Assets/Scenes/SnackLine_TP.unity`. Elle contient seulement le sol.

## Organisation du projet

| Dossier | Contenu |
|---|---|
| `Scripts/TP/` | **Les scripts à compléter** : `TransformerMachine` et `CombinerMachine`. |
| `Scripts/Core/` | Le moteur du TP (pas besoin de le modifier) : `Machine`, `Conveyor`, `ItemSpawner`, `SellZone`, `Item`. |
| `Prefabs/Items/` | 200 aliments. Le nom du prefab est le nom de l'item (ex : `meat-raw`, `bread`, `burger`). |
| `Prefabs/Machines/` | Convoyeurs, virages, source (`Spawner`), vente (`SellZone`) et `Machine_Modele`. |
| `Kenney/FactoryKit/` | Modèles 3D pour décorer votre usine (murs, passerelles, robots, écrans...). |

Cherchez `TODO` dans le dossier `Scripts/TP` pour trouver le code à écrire.

---

## Comment marche une machine

Toutes les machines (même les convoyeurs) héritent de la classe `Machine`. Une machine est un objet avec 3 enfants importants :

| Enfant | Rôle |
|---|---|
| `InputZone` | Un Box Collider : les items qui le touchent **entrent** dans la machine. |
| `OutputPoint` | Un objet vide : les items **sortent** à cet endroit. |
| `ProgressBar` | La barre verte qui se remplit pendant le travail, avec `SetProgressBar(...)`. |

Le travail se passe en 3 temps. Dans votre machine, vous réécrivez (`override`) ces 3 fonctions pour dire ce que fait la machine à chaque temps :

| Fonction | Quand ? |
|---|---|
| `OnItemEnter(Item item)` | Un item vient d'entrer dans la machine. |
| `OnProgress(float progress)` | À chaque image pendant le travail. `progress` va de 0 (début) à 1 (fin). |
| `OnEnd()` | Le travail est fini : la machine produit quelque chose. Ensuite, la liste `items` est vidée automatiquement. |

Les outils disponibles dans une machine :

| Outil | Rôle |
|---|---|
| `items` | La liste des items dans la machine. `items[0]` est le premier. |
| `CreateItem(prefab)` | Crée un nouvel item dans la machine et le renvoie. |
| `Output(item)` | Fait sortir un item sur l'`OutputPoint`. |
| `SetProgressBar(valeur)` | Règle la barre de progression : 0 = vide, 1 = pleine. |
| `Destroy(item.gameObject)` | Détruit un item (fonction de Unity). |
| `item.gameObject.SetActive(false)` | Cache un item (fonction de Unity). |
| `GetItem("bread")` | Renvoie l'item de la machine qui porte ce nom (ou `null`). |

Les réglages dans l'Inspector :

| Champ | Rôle |
|---|---|
| `Progress Time` | Durée du travail, en secondes. |
| `Accepted Items` | La **recette** : les noms des items acceptés. La machine démarre quand elle les a **tous**. Vide = accepte tout, un item à la fois. |

> ⚠️ **Règle d'or** : l'`OutputPoint` d'une machine doit toucher l'`InputZone` de la machine suivante. Sinon, la machine se bloque et la Console vous prévient.
>
> ⚠️ N'écrivez **pas** de fonction `Awake`, `Update` ou `OnTriggerStay` dans vos machines : elles remplaceraient celles de `Machine`, et plus rien ne marcherait.

### Placer les machines

- L'usine est une **grille de cases de 1 m**. Placez les machines sur des positions entières (X = 2, Z = -3...) : le plus simple est de taper les valeurs directement dans le composant Transform.
- Un convoyeur avance dans le sens de la **flèche bleue** (axe Z). Pour le tourner, changez la **Rotation Y** : 0, 90, 180 ou -90.
- Les virages : `Conveyor_CornerRight` tourne à droite, `Conveyor_CornerLeft` tourne à gauche.
- Dans la vue Scene, la ligne bleu clair montre le trajet de chaque convoyeur, et la sphère jaune l'`OutputPoint` des machines.

---

## Étape 1 : Découverte

Dans `SnackLine_Exemple` :

1. Sélectionnez le `Spawner` et changez son `Item Prefab` : glissez un autre aliment depuis `Prefabs/Items`. Lancez Play.
2. Changez son `Interval`, puis le `Progress Time` d'un convoyeur. Qu'est-ce qui change ?
3. Ajoutez un convoyeur pour rallonger la ligne, sans rien casser.

## Étape 2 : Le four (`TransformerMachine`)

Le four transforme un steak cru (`meat-raw`) en steak cuit (`meat-cooked`).

1. **Le code** : complétez les 7 TODO de `TransformerMachine`. Chaque TODO vous donne la ligne à écrire.
2. **La machine** :
   1. Glissez `Prefabs/Machines/Machine_Modele` dans la scène `SnackLine_TP` et renommez-le `Four`.
   2. `Add Component` → `TransformerMachine`.
   3. Glissez les enfants `InputZone`, `OutputPoint` et `ProgressBar` dans les champs du même nom.
   4. `Accepted Items` : ajoutez une ligne `meat-raw`. `Result Prefab` : glissez `Prefabs/Items/meat-cooked`.
3. **Le prefab** : glissez `Four` depuis la Hierarchy vers le dossier `Prefabs/Machines`. Choisissez **Original Prefab**.
4. **Le test** : Spawner (`meat-raw`) → convoyeurs → Four → convoyeurs → SellZone. La Console doit afficher `Vendu : meat-cooked`.

## Étape 3 : L'assembleur (`CombinerMachine`)

L'assembleur combine plusieurs ingrédients en un produit (ex : `bread` + `meat-cooked` → `burger`).

1. Complétez les 7 TODO de `CombinerMachine`. Cette fois, la ligne à écrire n'est pas donnée : inspirez-vous de votre four ! Attention, il faut détruire **tous** les ingrédients (boucle `foreach`).
2. Créez la machine et son prefab comme à l'étape 2, avec `Accepted Items` = `bread` et `meat-cooked`.
3. Amenez les deux ingrédients à l'assembleur par **deux convoyeurs différents**, qui arrivent chacun par un côté de la machine.

## Étape 4 : Variante améliorée (Prefab Variant)

Une Prefab Variant est une copie d'un prefab qui garde un lien avec l'original, mais avec quelques réglages différents.

1. Clic droit sur votre prefab `Four` → `Create` → `Prefab Variant`. Nommez-le par exemple `Four_Turbo`.
2. Améliorez-le : `Progress Time` plus court, autre modèle 3D, autre couleur, autre recette...
3. Utilisez-le dans votre usine.

## Étape 5 : Votre usine

Construisez dans `SnackLine_TP` une usine **complète et cohérente** :

- au moins une chaîne qui va des sources jusqu'à la vente, **sans intervention** ;
- vos deux machines utilisées ;
- un décor crédible avec le `FactoryKit` : murs (`structure-*`), passerelles (`catwalk-*`), robots, écrans, tuyaux... Pas d'objets qui flottent ou qui se traversent.

Recettes possibles avec les aliments du Food Kit :

| Ingrédients | Produit |
|---|---|
| `meat-raw` | `meat-cooked` |
| `hot-dog-raw` | `hot-dog` |
| `bread` + `meat-cooked` | `burger` |
| `burger` + `cheese-cut` | `burger-cheese` |
| `donut` + `chocolate` | `donut-chocolate` |
| ... | à vous d'inventer ! |

---

## Bonus

- **3e machine** : une machine de votre invention (un trieur, une machine qui emballe, qui découpe...).
- **VFX et matériau / shader** : un effet de particules quand la machine travaille ou termine, un item qui change de couleur...
- **Prefabs propres** : chaque machine est un prefab, et la scène n'utilise que des instances de prefabs.
- **Compteur de production** : un texte à l'écran « Burgers produits : 12 ». Le nombre de ventes est dans la variable `totalSold` de `SellZone`.
- **Objectif de production** : une commande à remplir (« 10 burgers ») qui affiche un message quand elle est terminée.
- **Rendu portfolio** : une vidéo de 30 s de votre usine (`Window` → `General` → `Recorder` → `Recorder Window` → `Add Recorder` → `Movie`), et une belle capture d'écran.
- **Idée en plus** : éclairage soigné, animations, caméra de présentation...

## Barème (/20)

| Critère | Points |
|---|---|
| Machine qui transforme | 2,5 |
| Machine qui combine | 2,5 |
| Variante de machine améliorée (Prefab Variant) | 2 |
| Chaîne complète de la source à la vente, sans intervention | 2 |
| Level art cohérent | 2 |
| **Bonus** : 3e machine | 2 |
| **Bonus** : VFX et changement de matériau / shader | 2 |
| **Bonus** : prefabs propres | 1 |
| **Bonus** : compteur de production (UI) | 1 |
| **Bonus** : objectif de production avec message de fin | 1 |
| **Bonus** : rendu portfolio (vidéo + capture) | 1 |
| **Bonus** : idée supplémentaire pertinente | 1 |

---

## Ça ne marche pas ?

### Une erreur rouge dans la Console

- **On ne peut pas lancer Play tant qu'il reste une erreur rouge.** Double-cliquez sur l'erreur : elle vous emmène à la ligne du problème.
- `error CS1513: } expected` ou `CS1022` : il manque une accolade `}` (ou il y en a une de trop). Chaque `{` doit avoir sa `}`.
- `error CS1002: ; expected` : il manque un `;` à la fin d'une ligne.

### `NullReferenceException`

Ça veut dire : **« tu utilises quelque chose qui est vide »**. Dans 9 cas sur 10, c'est un champ de l'Inspector resté à `None`.

1. Double-cliquez sur l'erreur pour trouver la ligne.
2. Regardez quelle variable est utilisée sur cette ligne.
3. Vérifiez dans l'Inspector que ce champ est bien rempli (glisser-déposer l'objet ou le prefab).

### Les items ne bougent pas ou la chaîne s'arrête

Lisez la Console : les machines écrivent un message quand quelque chose ne va pas. Sinon, vérifiez :

- Les champs `Input Zone`, `Output Point` (et `Progress Bar`) de la machine sont remplis.
- L'`OutputPoint` (sphère jaune) touche l'`InputZone` de la machine suivante.
- Les convoyeurs sont tournés dans le bon sens (flèche bleue).
- `Accepted Items` contient **exactement** le nom de l'item (`meat-raw`, pas `Meat Raw`).
- `OnEnd` fait bien sortir quelque chose avec `Output(...)`.

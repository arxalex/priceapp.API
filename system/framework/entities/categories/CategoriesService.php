<?php

namespace framework\entities\categories;

use framework\database\ListHelper;
use framework\database\StringHelper;
use framework\entities\default_entities\DefaultEntitiesService;
use framework\entities\categories\Category;

class CategoriesService extends DefaultEntitiesService
{
    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "categories\\Category";
        $this->tableName = "pa_categories";
    }

    public function getCategoryByName(string $str, ?Category $base = null): Category
    {
        $str = mb_strtolower($str, 'UTF-8');
        $categories = $this->getItemsFromDB();
        if ($base !== null) {
            $categories = $this->filterCategories($categories, $base);
        }
        $result = array();
        foreach ($categories as $category) {
            $label = mb_strtolower($category->label);
            if (StringHelper::stringContains($str, substr($label, 0, -1)) === true) {
                $result[] = $category;
            }
        }

        if (count($result) >= 1) {
            $chains = array();
            $parents = array();
            foreach ($result as $category) {
                $parents[] = $category->parent;
            }
            foreach ($result as $category) {
                if (!in_array($category->id, $parents)) {
                    $chainTemp = array();
                    $chainTemp[] = $category->id;
                    while ($chainTemp[count($chainTemp) - 1] != $base->id) {
                        $found = false;
                        foreach ($categories as $v2) {
                            if ($v2->id == $chainTemp[count($chainTemp) - 1]) {
                                $chainTemp[] = $v2->parent != NULL ? $v2->parent : $base->id;
                                $found = true;
                                break;
                            }
                        }
                        if (!$found) {
                            $chainTemp[] = $base->id;
                        }
                    }
                    $chains[] = $chainTemp;
                }
            }

            if (count($chains) == 1) {
                return $this->getItemFromDB($chains[0][0]);
            }
            $resultReturn = 0;
            $max = 0;
            foreach ($chains as $chain) {
                if (count($chain) > $max) {
                    $resultReturn = $chain[0];
                    $max = count($chain);
                }
            }
            $category = $this->getItemFromDB($resultReturn);
            foreach ($chains as $chain) {
                if (count($chain) == $max) {
                    $categoryTemp = $this->getItemFromDB($chain[0]);
                    if (strlen($categoryTemp->label) > strlen($category->label)) {
                        $category = $categoryTemp;
                        $resultReturn = $chain[0];
                    }
                }
            }
            return $this->getItemFromDB($resultReturn);
        } else {
            return $base == null ? $this->getItemFromDB(0) : $base;
        }
    }

    public function filterCategories(array $categories, Category $parent): array
    {
        $result = array();
        foreach ($categories as $category) {
            if ($category->id == $parent->id) {
                $result[] = $category;
            } elseif ($category->parent == $parent->id) {
                $result[] = $category;
            } else {
                $chainTemp = array();
                $chainTemp[] = $category->id;
                $chainTemp[] = $category->parent;
                while ($chainTemp[count($chainTemp) - 1] != -1) {
                    $found = false;
                    foreach ($categories as $v2) {
                        if ($v2->id == $chainTemp[count($chainTemp) - 1]) {
                            $chainTemp[] = $v2->parent != NULL ? $v2->parent : -1;
                            $found = true;
                            break;
                        }
                    }
                    if (!$found) {
                        $chainTemp[] = -1;
                    }
                    if ($chainTemp[count($chainTemp) - 1] == $parent->id) {
                        $result[] = $category;
                        break;
                    }
                }
            }
        }
        return $result;
    }

    public function getCategoriesByParent(int $parentId): array
    {
        $resultByLevel = [];
        $i = 0;
        $resultByLevel[$i] = [0 => $parentId];
        do {
            $i++;
            $resultByLevel[$i] = ListHelper::getColumn($this->getItemsFromDB([
                'parent' => $resultByLevel[$i - 1]
            ]), 'id');
        } while (count($resultByLevel[$i - 1]) > 0);

        $result = [];

        foreach ($resultByLevel as $value) {
            $result = array_merge($result, $value);
        }

        return $result;
    }
}

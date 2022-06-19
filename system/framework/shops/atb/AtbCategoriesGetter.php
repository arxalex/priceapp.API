<?php

namespace framework\shops\atb;

use framework\entities_proxy\categories\AtbCategoriesService;
use framework\shops\atb\AtbCategoriesModel;

class AtbCategoriesGetter
{
    private AtbCategoriesService $_atbCategoriesService;
    public function __construct()
    {
        $this->_atbCategoriesService = new AtbCategoriesService();
    }
    public function get(int $fillialId = 310): array
    {
        $preCategories = $this->_atbCategoriesService->getItemsFromDB();

        $categories = [];
        foreach ($preCategories as $value) {
            $categories[] = new AtbCategoriesModel(
                $value->id,
                $value->label
            );
        }
        return $categories;
    }
}

<?php

namespace businessLogic\prices;

use framework\entities\filials\FilialsService;
use viewModels\PriceWithFilialViewModel;

class PricesWebService
{
    private FilialsService $_filialsService;

    public function __construct()
    {
        $this->_filialsService = new FilialsService();
    }

    public function getPricesWithFilialsViewModelByItemIdAndCord(int $id, float $xCord, float $yCord, int $radius): array
    {
        $filials = $this->_filialsService->getFilialsByCord($xCord, $yCord, $radius);
        $result = [];
        echo var_dump($filials);
        foreach ($filials as $filial) {
            $model = new PriceWithFilialViewModel($id, $filial);
            echo var_dump($model);

            if ($model->price > 0) {
                $result[] = $model;
            }
        }
        return $result;
    }
}

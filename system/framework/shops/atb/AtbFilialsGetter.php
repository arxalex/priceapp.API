<?php

namespace framework\shops\atb;

use framework\database\NumericHelper;
use framework\entities\filials\Filial;
use framework\entities\filials\FilialsService;
use framework\entities_proxy\filials\AtbFilialsService;

class AtbFilialsGetter
{
    private FilialsService $_fillialsService;
    private AtbFilialsService $_atbFillialsService;

    public function __construct()
    {
        $this->_fillialsService = new FilialsService();
        $this->_atbFillialsService = new AtbFilialsService();
    }

    public function getFilials(): array {
        return $this->_atbFillialsService->getItemsFromDB();
    }
    public function convertFilial(object $filialAtb): Filial
    {
        return new Filial(
            null,
            3,
            $filialAtb->id,
            $filialAtb->city,
            $filialAtb->region,
            $filialAtb->street,
            $filialAtb->house,
            NumericHelper::toFloat($filialAtb->xcord),
            NumericHelper::toFloat($filialAtb->ycord),
            $filialAtb->label
        );
    }
}

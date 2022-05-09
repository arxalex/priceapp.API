<?php

namespace endpoint\filials;

use endpoint\defaultBuild\BaseEndpointBuilder;
use framework\database\ListHelper;
use framework\entities\filials\FilialsService;
use framework\shops\silpo\SilpoFilialsGetter;

class GetFilials extends BaseEndpointBuilder
{
    private FilialsService $_filialsService;
    private SilpoFilialsGetter $_silpoFilialsGetter;
    public function __construct()
    {
        parent::__construct();
        $this->_filialsService = new FilialsService();
        $this->_silpoFilialsGetter = new SilpoFilialsGetter();
    }
    public function defaultParams()
    {
        return [
            'shopid' => null,
            'source' => 0,
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRequest($this->getParam('cookie'), 1);
        if ($this->getParam('source') === 0) {
            if ($this->getParam('shopid') === null) {
                $filials = $this->_filialsService->getItemsFromDB();
            } else {
                $filials = $this->_filialsService->getItemsFromDB(['shopid' => [$this->getParam('shopid')]]);
            }
            return $filials;
        } elseif ($this->getParam('source') === 1) {
            $this->_usersService->unavaliableRequest($this->getParam('cookie'));
            if ($this->getParam('shopid') === 1) {
                $filialsInDb = $this->_filialsService->getItemsFromDB(['shopid' => [1]]);
                $silpoFilialModels = $this->_silpoFilialsGetter->getFilials();
                $filials = [];
                foreach ($silpoFilialModels as $model) {
                    $filial_temp = $this->_silpoFilialsGetter->convertFilial($model);
                    if (!ListHelper::isObjectinArray($filial_temp, $filialsInDb, ["inshopid"])) {
                        $this->_filialsService->insertItemToDB($filial_temp);
                        $filials[] = $filial_temp;
                    }
                }

                return true;
            }
        }
    }
}

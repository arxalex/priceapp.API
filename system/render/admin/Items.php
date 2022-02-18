<?php

namespace render\admin;

use render\defaultBuild\BaseRenderBuilder;

class Items extends BaseRenderBuilder
{
    public function getExternalParams()
    {
        return [
            'header_title' => 'Items',
            'scripts' => [
                '<script src="/system/scripts/admin/items.js"></script>',
                '<script src="/system/scripts/admin/item.js"></script>',
                '<script src="/system/scripts/admin/itemsaver.js"></script>'
            ]
        ];
    }
    public function defaultParams()
    {
        return [];
    }
    public function build()
    {

        return '<items></items>';
    }
}

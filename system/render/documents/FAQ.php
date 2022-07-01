<?php

namespace render\documents;

use render\defaultBuild\BaseRenderBuilder;

class FAQ extends BaseRenderBuilder
{
    public function defaultParams()
    {   
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        return '
            <h1>Часто задавані питання</h1>
            <ol>
                <li>
                    <b>Акаунт було зареєстровано, але вхід виконати не можна</b>
                    <div>
                        <p>
                            Переконайтесь, що ви підтвердили електронну пошту. Перевірте папки "Пропозіції" та "Спам".
                        </p>
                    </div>
                </li>
                <li>
                    <b>У списку немає потрібного мені товару</b>
                    <div>
                        <p>
                            Спробуйте збільшити радіус пошуку. Якщо товар досі відсутній, але ви впевнені, що він є у магазинах, що присутні у додатку - напишіть нам на пошту. Бажано також додати посилання на товари.
                        </p>
                    </div>
                </li>
            </ol>
            <p>
                Якщо у вас залишились питання - пишыть на пошту <a href="mailto:info@arxalex.co">info@arxalex.co</a>
            </p>';
    }
}

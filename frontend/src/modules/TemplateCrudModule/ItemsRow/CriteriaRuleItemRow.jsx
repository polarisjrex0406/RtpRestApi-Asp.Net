import { useRef } from 'react';
import { generate as uniqueId } from 'shortid';

import { Form, Input, Row, Col, Select, Divider, Button, Card } from 'antd';
import { PlusOutlined, DeleteOutlined } from '@ant-design/icons';

import useLanguage from '@/locale/useLanguage';

import ConditionsItemRow from './ConditionsItemRow';

export default function CriteriaRuleItemRow({ field, remove, current = null, varsInPrompt = [] }) {
  const translate = useLanguage();
  const addConditionsField = useRef(false);

  return (
    <Row gutter={[12, 12]} style={{ position: 'relative' }}>
      <Col className="gutter-row" span={24}>
        <Card>
          <Row gutter={[12, 12]} style={{ position: 'relative' }}>
            <Col className="gutter-row" span={16}>
              <Form.Item
                name={[field.name, 'ruleName']}
                rules={[
                  {
                    required: true,
                    message: 'Missing rule name',
                  },
                ]}
              >
                <Input placeholder="Rule name" />
              </Form.Item>
            </Col>
            <Col className="gutter-row" span={7}>
              <Form.Item name={[field.name, 'conditionsLogic']}
                rules={[
                  {
                    required: true,
                  },
                ]}
              >
                <Select
                  options={[
                    { value: 'Any', label: 'Any' },
                    { value: 'All', label: 'All' },
                  ]}
                ></Select>
              </Form.Item>
            </Col>
            <Form.List name={[field.name, 'conditions']}>
              {(subFields, subOpt) => (
                <>
                  {subFields.map((subField, index) => (
                    <ConditionsItemRow key={`${uniqueId()}`} remove={subOpt.remove} field={subField} current={current?.conditions[index]} varsInPrompt={varsInPrompt}>
                    </ConditionsItemRow>
                  ))}
                  <Form.Item>
                    <Button
                      type="dashed"
                      onClick={() => subOpt.add()}
                      block
                      icon={<PlusOutlined />}
                      ref={addConditionsField}
                    >
                      {translate('Add Conditions field')}
                    </Button>
                  </Form.Item>
                </>
              )}
            </Form.List>
          </Row>
        </Card>
        <div style={{ position: 'absolute', right: '-20px', top: ' 5px' }}>
          <DeleteOutlined onClick={() => remove(field.name)} />
        </div>
      </Col>
      <Divider />
    </Row>
  );
}

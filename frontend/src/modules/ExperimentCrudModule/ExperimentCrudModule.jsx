import { useLayoutEffect } from 'react';
import { useDispatch } from 'react-redux';

import { crud } from '@/redux/crud/actions';

import ExperimentCrudLayout from '@/layout/ExperimentCrudLayout/index'
import DeleteModal from '@/components/DeleteModal';
import CardBox from '@/components/ExperimentCardBox/CardBox';

function ExperimentCrudModule({ config, createForm, updateForm, withUpload = false }) {
  const dispatch = useDispatch();

  useLayoutEffect(() => {
    dispatch(crud.resetState());
  }, []);

  return (
    <ExperimentCrudLayout config={config}>
      <CardBox config={config} />
      <DeleteModal config={config} />
    </ExperimentCrudLayout>
  );
}

export default ExperimentCrudModule;
